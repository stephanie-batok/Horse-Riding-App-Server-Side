using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DATA.EF;
using WebApi.DTO;
using System.Web.Http.Cors;


namespace WebApi.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class HorseController : ApiController
    {
        // GET api/<controller>
        public List<HorseDTO> Get()
        {
            horseClubDbContext db = new horseClubDbContext();

            var horses = db.horses.Select(h => new HorseDTO()
            {
                id = h.id,
                name = h.name,
                gender = h.gender,
                size = h.size,
                temper = h.temper,
                is_active = h.is_active,
                required_rank = h.required_rank,
                max_weight = h.max_weight,
                min_weight = h.min_weight,
                max_height = h.max_height,
                min_height = h.min_height,
                therapeutic_riding = h.therapeutic_riding,
                can_jump = h.can_jump,
                is_qualified = h.is_qualified

            }).ToList();

            return horses;
        }

        // GET api/<controller>/5
        [Route("api/Horse/{horse_id}")]
        public HorseDTO Get(int horse_id)
        {
            horseClubDbContext db = new horseClubDbContext();

            HorseDTO horse = db.horses.Where(x => x.id == horse_id).Select(h => new HorseDTO()
            {
                id = h.id,
                name = h.name,
                gender = h.gender,
                size = h.size,
                temper = h.temper,
                is_active=h.is_active,
                required_rank = h.required_rank,
                max_weight = h.max_weight,
                min_weight = h.min_weight,
                max_height = h.max_height,
                min_height = h.min_height,
                therapeutic_riding = h.therapeutic_riding,
                can_jump = h.can_jump,
                is_qualified = h.is_qualified

            }).SingleOrDefault();

            return horse;
        }

        // POST api/<controller>
        public HttpResponseMessage Post([FromBody]HorseDTO horse)
        {
            horseClubDbContext db = new horseClubDbContext();

            if (string.IsNullOrEmpty(horse.name) || horse.name=="")
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "פרטי הסוס - יש להזין את שם הסוס");
            }

            if (string.IsNullOrEmpty(horse.gender) || (horse.gender!="זכר" && horse.gender!="נקבה"))
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "פרטי הסוס - יש להזין את מין הסוס (זכר או נקבה)");
            }

            if (horse.required_rank <=0 || horse.required_rank>5)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "מגבלות הסוס - יש להזין רמת רכיבה נדרשת בין 1 ל-5");
            }

            if (horse.min_weight !=null && horse.max_weight != null)
            {
                if (horse.min_weight<0 || horse.max_weight<0 || horse.min_weight>horse.max_weight)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "מגבלות הסוס - יש להזין משקל מינמאלי ומקסימאלי תקין");
                }
            }

            if (horse.min_height != null && horse.max_height != null)
            {
                if (horse.min_height < 0 || horse.max_height < 0 || horse.min_height > horse.max_height)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "מגבלות הסוס - יש להזין גובה מינמאלי ומקסימאלי תקין");
                }
            }

            if (horse.is_qualified is null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "מגבלות הסוס - יש להזין מצב רפואי נוכחי של הסוס");
            }

            horse h = db.horses.Where(x=> x.is_active==true).SingleOrDefault(x => x.name == horse.name);
            if (h!=null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "סוס בעל שם זהה קיים במערכת");
            }
            else
            {
                horse new_horse = new horse
                {
                    name = horse.name,
                    gender = horse.gender,
                    size = horse.size,
                    temper = horse.temper,
                    required_rank = horse.required_rank,
                    max_weight = horse.max_weight,
                    min_weight = horse.min_weight,
                    max_height = horse.max_height,
                    min_height = horse.min_height,
                    therapeutic_riding = horse.therapeutic_riding,
                    can_jump = horse.can_jump,
                    is_qualified = horse.is_qualified,
                    is_active = true
                };

                db.horses.Add(new_horse);
            }
            try
            {
                db.SaveChanges();

                return Request.CreateResponse(HttpStatusCode.OK, "הסוס התווסף בהצלחה");
            }
            catch (Exception)
            {
                return Request.CreateResponse(HttpStatusCode.NotImplemented, "שגיאה");
            }
        }

        // PUT api/<controller>/5
        [HttpPut]
        [Route("api/Horse/HorseDetails/{id}")]
        public HttpResponseMessage PutHorseDetails(int id, [FromBody]HorseDTO horse)
        {
            horseClubDbContext db = new horseClubDbContext();
            horse h = db.horses.SingleOrDefault(x => x.id == id);

            if (string.IsNullOrEmpty(horse.gender) || (horse.gender != "זכר" && horse.gender != "נקבה"))
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "פרטי הסוס - יש להזין את מין הסוס (זכר או נקבה)");
            }

            if (h!=null)
            {
                h.gender = horse.gender;
                h.size = horse.size;
                h.temper = horse.temper;
                h.is_active = horse.is_active;
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "פרטי הסוס לא נמצאו במערכת - נסה שנית");
            }

            try
            {
                db.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.OK, "פרטי הסוס עודכנו בהצלחה");
            }
            catch (Exception)
            {
                return Request.CreateResponse(HttpStatusCode.NotImplemented, "שגיאה");
            }
        }

        // PUT api/<controller>/5
        [HttpPut]
        [Route("api/Horse/HorseRestrictions/{id}")]

        public HttpResponseMessage PutHorseRestrictions(int id, [FromBody]HorseDTO horse)
        {
            horseClubDbContext db = new horseClubDbContext();
            horse h = db.horses.SingleOrDefault(x => x.id == id);

            if (horse.required_rank <= 0 || horse.required_rank > 5)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "מגבלות הסוס - יש להזין רמת רכיבה נדרשת בין 1 ל-5");
            }

            if (horse.min_weight != null && horse.max_weight != null)
            {
                if (horse.min_weight < 0 || horse.max_weight < 0 || horse.min_weight > horse.max_weight)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "מגבלות הסוס - יש להזין משקל מינמאלי ומקסימאלי תקין");
                }
            }

            if (horse.min_height != null && horse.max_height != null)
            {
                if (horse.min_height < 0 || horse.max_height < 0 || horse.min_height > horse.max_height)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "מגבלות הסוס - יש להזין גובה מינמאלי ומקסימאלי תקין");
                }
            }

            if (horse.is_qualified is null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "מגבלות הסוס - יש להזין מצב רפואי נוכחי של הסוס");
            }

            if (h != null)
            {
                h.required_rank = horse.required_rank;
                h.max_weight = horse.max_weight;
                h.min_weight = horse.min_weight;
                h.max_height = horse.max_height;
                h.min_height = horse.min_height;
                h.therapeutic_riding = horse.therapeutic_riding;
                h.can_jump = horse.can_jump;
                h.is_qualified = horse.is_qualified;
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "פרטי הסוס לא נמצאו במערכת - נסה שנית");
            }

            try
            {
                db.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.OK, "פרטי הסוס עודכנו בהצלחה");
            }
            catch (Exception)
            {
                return Request.CreateResponse(HttpStatusCode.NotImplemented, "שגיאה");
            }
        }

        // DELETE api/<controller>/5
        [HttpDelete]
        [Route("api/Horse/{id}")]
        public HttpResponseMessage Delete(int id)
        {
            horseClubDbContext db = new horseClubDbContext();
            horse h = db.horses.SingleOrDefault(x => x.id == id);

            if (h != null)
            {
                h.is_active = false;
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, " הסוס לא נמצא במערכת");
            }

            try
            {
                db.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.OK, "הסוס נמחק בהצלחה");
            }
            catch (Exception)
            {
                return Request.CreateResponse(HttpStatusCode.NotImplemented, "שגיאה");
            }
        }
    }
    
}