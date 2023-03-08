using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore.InMemory;
using AddressProject.DB;
using AddressProject.Entities;
using AddressProject.Models;
using AddressProject.Converter;
using System.Text;
using System.Linq.Dynamic;
using System.Linq.Dynamic.Core;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using AutoMapper;
using System.ComponentModel;
using Microsoft.Extensions.Logging;

namespace AddressProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddressesController : ControllerBase
    {
        private readonly AddressDataContex _context;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AddressesController> _logger;
        public AddressesController(AddressDataContex context, IMapper mapper
            ,IConfiguration configuration,ILogger<AddressesController> logger)
        {
            _context = context;
            _mapper = mapper;
            _configuration = configuration;
            _logger = logger;
        }
        //send address obj and bring uri (of location)
        private string getApiUrl(Address address)
        {
            string apiurl = _configuration["apiurl"];
            string apikey = _configuration["apikey"];
            var query = new Dictionary<string, string>()
            {
                ["q"] = $"{address.HouseNumber},{address.Street},{address.City},{address.Country},{address.ZipCode}",
                ["key"]= apikey,
                ["format"]= "json"
            };
            var uri=QueryHelpers.AddQueryString(apiurl, query);
            return uri;
        }
        //GetPositionAsync recieve address obj and by using api bring
        //the position(s) of address(with positionDTO type) and we select the first of them and 
        //define a position obj and send its lat and lon of addressDTO 
        //to lat and lon of position obj
        //the positionDTO is the class similar to location api
        //which we want lat and lon properties of it.
        private async Task<Position?> GetPositionAsync(Address address)
        {
            //send address obj and bring uri and by http recieving the response
            //in response content we can access to the ge0-address as positionDTO obj
            var url=getApiUrl(address);
            var http=new HttpClient();
            var resp= await http.GetAsync(url);
            var positions=await resp.Content.ReadFromJsonAsync<PositionDTO[]>();
            var p = positions[0];
            return new Position { lat = double.Parse(p.lat), lon = double.Parse(p.lon) };
        }

        // GET: api/Addresses
        [HttpGet]
        
        public async Task<ActionResult<IEnumerable<AddressDTO>>> GetAddress()
        {
            if (_context.Address == null)
            {
                
                return NotFound();
            }
            try
            {
                var address = await _context.Address.ToListAsync();
                var adrs = _mapper.Map<List<AddressDTO>>(address);
                return Ok(adrs);
            }
            catch (Exception e) 
            {
                _logger?.LogError(e, "GetAddress error!");
                return BadRequest(e.Message);
            }
            
        }
    
   
        //GET with Filter
        [HttpGet("Filter")]
        public async Task<ActionResult<IEnumerable<AddressDTO>>> GetAddressFilter
              ([FromQuery] AddressQuery addressQuery, [FromQuery] SortOption sort)
        {
            try
            {
                if (sort.SortOrder != null ^ sort.SortBy != null)
                    return BadRequest();
                var filterValues = new List<object>();
                var sb = new StringBuilder();
                int valueId = 0;
                //get the values were filled and send to filterValues variable
                foreach (var p in (typeof(AddressQuery).GetProperties()))
                {
                    var fieldValue = p.GetValue(addressQuery);
                    if (fieldValue != null)
                    {
                        sb.Append($"{p.Name}=@{valueId++}&&");
                        filterValues.Add(fieldValue);
                    }

                }
                //remove last &&
                if (sb.Length > 0) sb.Remove(sb.Length - 2, 2);
                var fstring = sb.ToString();

                IQueryable<Address> query = _context.Address;

                //if at least one of the fields is filled
                if (fstring != null)
                    query = query.Where(fstring, filterValues.ToArray());
                if (sort.SortBy != null && sort.SortOrder != null)
                    query = query.OrderBy($"{sort.SortBy} {sort.SortOrder}");

                // var adrs = await query.Select(x => x.ToAddressDTO()).ToListAsync();
                var address = await query.ToListAsync();
                var adrs = _mapper.Map<List<AddressDTO>>(address);
                return Ok(adrs);
            }
            catch (Exception e)
            {
                _logger?.LogError(e, "GetAddressFilter error!");
                return BadRequest(e.Message);
            }
        }
        //GET with Search
        [HttpGet("Search")]
        public async Task<ActionResult<IEnumerable<AddressDTO>>> GetAddressFilterByString
              (String search)
        {
            try
            {
                IQueryable<Address> query = _context.Address;
                //Convert by Coverter class functions
                // var adrs = await query.Select(x => x.ToAddressDTO()).ToListAsync();
                var address = await query.ToListAsync();

                if (search != null)
                {
                    var anysb = new StringBuilder();
                    foreach (var p in (typeof(Address).GetProperties()))
                        if (p.PropertyType == typeof(string))
                            anysb.Append($"{p.Name}.Contains(@0)||");
                    //remove last ||
                    if (anysb.Length > 0)
                        anysb.Remove(anysb.Length - 2, 2);

                    query = query.Where(anysb.ToString(), search);
                }
                //Convert by Converter class functions
                // adrs = await query.Select(x => x.ToAddressDTO()).ToListAsync();
                //Convert by AutoMapper
                address = await query.ToListAsync();
                var adrs = _mapper.Map<List<AddressDTO>>(address);
                return Ok(adrs);
            }
            catch (Exception e)
            {
                _logger?.LogError(e, "GetAddressFilterByString error!");
                return BadRequest(e.Message);
            }
        }

        // GET: api/Addresses/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AddressDTO>> GetAddress(int id)
        {
            try
            {
                if (_context.Address == null)
                {
                    return NotFound();
                }
                var address = await _context.Address.FindAsync(id);

                if (address == null)
                {
                    return NotFound();
                }
                //Convert by Converter class functions
                // return address.ToAddressDTO();
                //Convert by AutoMapper
                var adrs = _mapper.Map<AddressDTO>(address);
                return Ok(adrs);
            }
            catch (Exception e)
            {
                _logger?.LogError(e, "GetAddress error!");
                return BadRequest(e.Message);
            }
        }
        //GET ("distance")
        [HttpGet("Distance")]

        public async Task<ActionResult<double>> GetDistance(int id1, int id2)
        {
            if (_context.Address == null)
            {
                return NotFound();
            }
            try
            {
                var adrs1 = await _context.Address.FindAsync(id1);
                var adrs2 = await _context.Address.FindAsync(id2);

                if (adrs1 == null || adrs2 == null)
                {
                    return NotFound();
                }
                var pos1 = GetPositionAsync(adrs1);
                var pos2= GetPositionAsync(adrs2);
                Task.WaitAll(pos1, pos2);
                return Ok(Position.distance(pos1.Result,pos2.Result));

            }
            catch  (Exception e)
            {
                _logger?.LogError(e, "in distance error!");
                return NotFound();
            }
        }

        // PUT: api/Addresses/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAddress(int id, AddressDTO addressDTO)
        {
            
            if (id != addressDTO.Id)
            {
                return BadRequest();
            }
            //Convert by AutoMapper
            var adrs = _mapper.Map<Address>(addressDTO);
            _context.Entry(adrs).State = EntityState.Modified;
            //Convert by Converter class functions
            // _context.Entry(addressDTO.ToAddress()).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException e)
            {
                if (!AddressExists(id))
                {
                    _logger?.LogError(e, "Address is not exist!");
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Addresses
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<AddressDTO>> PostAddress(AddressDTO addressDTO)
        {
            try
            {
                if (_context.Address == null)
                {
                    return Problem("Entity set 'DataContex.Address'  is null.");
                }
                //Convert By AutoMapper
                var address = _mapper.Map<Address>(addressDTO);

                //Convert by Converter class functions
                //var address = addressDTO.ToAddress(); 
                _context.Address.Add(address);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetAddress", new { id = address.Id }, address);
            }
            catch (Exception e)
            {
                _logger?.LogError(e, "PostAddress error!");
                return BadRequest(e.Message);
            }
        }

        // DELETE: api/Addresses/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAddress(int id)
        {
            try
            {
                if (_context.Address == null)
                {
                    return NotFound();
                }
                var address = await _context.Address.FindAsync(id);
                if (address == null)
                {
                    return NotFound();
                }

                _context.Address.Remove(address);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception e)
            {
                _logger?.LogError(e, "DeleteAddress error!");
                return BadRequest(e.Message);
            }
        }

        private bool AddressExists(int id)
        {
            return (_context.Address?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
