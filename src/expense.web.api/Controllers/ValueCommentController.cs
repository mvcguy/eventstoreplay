using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using expense.web.api.Values.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace expense.web.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValueCommentController : ControllerBase
    {
        // GET: api/ValueComment
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/ValueComment/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/ValueComment
        [HttpPost]
        public void Post([FromBody] CommentViewModel comment)
        {

        }

        // PUT: api/ValueComment/5
        [HttpPut("{parentId}/{id}")]
        public void Put(Guid? parentId, Guid? id, [FromBody] CommentViewModel comment)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
