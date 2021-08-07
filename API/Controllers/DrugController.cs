using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Data;
using Data.Models;
using Data.Models.Dtos;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NLog;
using RestSharp;
using RestSharp.Authenticators;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DrugController : ControllerBase
    {
        protected static Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly PharmacyDatabaseContext _dbContext;

        public DrugController(PharmacyDatabaseContext context)
        {
            _dbContext = context;
        }

        [HttpGet]
        public IActionResult Get(string token, string? code, string? label)
        {
            if (!VerifyToken(token))
            {
                return Unauthorized();
            }
            var drugList = _dbContext.Drugs?.ToList().Where(x => x.IsMatch(code, label));
            Logger.Info($"Loaded {drugList.Count()} Drugs");
            return Ok(drugList);
        }

        private bool VerifyToken(string token)
        {
            try
            {
                var tokenDto = new TokenVerificationDto();
                tokenDto.Token = token;
                var client = new RestClient("https://localhost:5001");
                client.RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;

                var request = new RestRequest("/api/Auth/verify", Method.POST);
                request.AddParameter("application/json; charset=utf-8", tokenDto.ToJson(), ParameterType.RequestBody);
                request.RequestFormat = DataFormat.Json;
                var response = client.Execute(request).Content ;
                dynamic json = JsonConvert.DeserializeObject(response) ;
                return bool.Parse(json.ValidToken);
            }
            catch (Exception e)
            {
                Logger.Error(e, "Could not verify token");
            }
            return false;
        }

        [HttpPost]
        public async Task<IActionResult> Add(DrugCreationDto dto)
        {
            try
            {
                if (!VerifyToken(dto.Token))
                {
                    return Unauthorized();
                }
                var drug = Drug.NewInstance(dto);
                _dbContext.Drugs?.Add(drug);
                await _dbContext.SaveChangesAsync();
                return StatusCode(201, new { id = drug.Id, message = "Drug Created" });
            }
            catch (Exception e)
            {
                Logger.Error(e, "Could not create drug");
            }

            return BadRequest();
        }

        [HttpPut]
        public async Task<IActionResult> Update(DrugUpdateDto dto)
        {
            try
            {
                if (!VerifyToken(dto.Token))
                {
                    return Unauthorized();
                }
                var count = _dbContext.Drugs.Count(x => x.Id == dto.Id);
                if (count == 0) return BadRequest($"there's no existing drug with ID = {dto.Id}");
                var drug = Drug.NewInstance(dto);
                _dbContext.Drugs?.Update(drug);
                await _dbContext.SaveChangesAsync();
                return StatusCode(200, new { id = drug.Id, message = "Drug Updated" });
            }
            catch (Exception e)
            {
                Logger.Error(e, $"Could not update drug with ID {dto.Id}");
            }

            return BadRequest();
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(string token, int id)
        {
            try
            {
                if (!VerifyToken(token))
                {
                    return Unauthorized();
                }
                var drug = _dbContext.Drugs.First(x => x.Id == id);
                _dbContext.Drugs?.Remove(drug);
                await _dbContext.SaveChangesAsync();
                return StatusCode(200, new { id = drug.Id, message = "Drug Deleted" });
            }
            catch (Exception e)
            {
                Logger.Error(e, $"Could not delete drug with ID {id}");
            }
            return BadRequest();
        }
    }
}