using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory;
using AddressProject.DB;
using AddressProject.Entities;
using AddressProject.Models;
using AddressProject.Converter;
using System.Text;
using System.Linq.Dynamic;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using AutoMapper;

namespace AddressProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddressesController : ControllerBase
    {
        private readonly AddressDataContex _context;
        private readonly IMapper _mapper;
        public AddressesController(AddressDataContex context,IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/Addresses
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AddressDTO>>> GetAddress()
        {
          if (_context.Address == null)
          {
              return NotFound();
          }
            var address=await _context.Address.ToListAsync();
            var adrs= _mapper.Map<List<AddressDTO>>(address);
            return Ok(adrs);
        }
        //GET with Filter
        [HttpGet("Filter")]
        public async Task<ActionResult<IEnumerable<AddressDTO>>> GetAddressFilter
              ([FromQuery] AddressQuery addressQuery, [FromQuery] SortOption sort)
        {
            
            if (sort.SortOrder!=null ^ sort.SortBy != null)
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
                query = query.Where(fstring,filterValues.ToArray());
            if (sort.SortBy != null && sort.SortOrder!=null)
                query = query.OrderBy($"{sort.SortBy} {sort.SortOrder}");
            
           // var adrs = await query.Select(x => x.ToAddressDTO()).ToListAsync();
           var address= await query.ToListAsync();
            var adrs=_mapper.Map<List<AddressDTO>>(address);
            return Ok(adrs);
        }
        //GET with Search
        [HttpGet("Search")]
        public async Task<ActionResult<IEnumerable<AddressDTO>>> GetAddressFilter
              (String search)
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

        // GET: api/Addresses/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AddressDTO>> GetAddress(int id)
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
            catch (DbUpdateConcurrencyException)
            {
                if (!AddressExists(id))
                {
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

        // DELETE: api/Addresses/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAddress(int id)
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

        private bool AddressExists(int id)
        {
            return (_context.Address?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
