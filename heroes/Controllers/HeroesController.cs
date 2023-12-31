﻿using heroes.Models;
using heroes.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace heroes.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class HeroesController : ControllerBase
    {
        private readonly IHeroRepository _heroRepository;
        public HeroesController(IHeroRepository heroRepository)
        {
            _heroRepository = heroRepository;
        }
        [HttpPatch("train/{heroId:guid}")]
        public async Task<IActionResult> Train([FromRoute] Guid heroId)
        {
            string? userId = User?.Identity?.Name;
            if (String.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized("you didn't send a token");
            }
            HeroResponseModel res = await _heroRepository.TryTrain(userId, heroId);
            return SendResponse(res, res => Ok(res.Heroes?[0]));
        }
        [HttpPatch("own/{heroId:guid}")]
        public async Task<IActionResult> AddHeroToUser([FromRoute] Guid heroId)
        {
            string? userId = User.Claims.ToList()[0].Value;
            if (String.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized("you didn't send a token");
            }
            HeroResponseModel res = await _heroRepository.AddHeroToUser(userId, heroId);
            return SendResponse(res);
        }
        [HttpPatch("unown/{heroId:guid}")]
        public async Task<IActionResult> RemoveHeroFromUser([FromRoute] Guid heroId)
        {
            string? userId = User.Claims.ToList()[0].Value;
            if (String.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized("you didn't send a token");
            }
            HeroResponseModel res = await _heroRepository.RemoveHeroFromUser(userId, heroId);
            return SendResponse(res);
        }
        [HttpGet("{page:int:min(1)}")]
        public async Task<IActionResult> GetAllHeroes([FromRoute] int page)
        {
            int amountOfHeroInPage = 3;
            string userId = User.Claims.ToList()[0].Value;
            if (String.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized("you didn't send a token");
            }
            HeroResponseModel res = await _heroRepository.GetAllHeroes(userId);
            return SendResponse(res, res =>{
                return Ok(
                    new{
                            amountOfHeroes = res.Heroes?.Count,
                            heroes = res.Heroes?.GetRange((page - 1) * amountOfHeroInPage, amountOfHeroInPage)
                        }
                    );
            });
        }
        [HttpGet("{heroId:guid}")]
        public async Task<IActionResult> GetHero([FromRoute] Guid heroId)
        {
            string? userId = User.Claims.ToList()[0].Value;
            if (String.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized("you didn't send a token");
            }
            HeroResponseModel res = await _heroRepository.GetHero(userId, heroId);
            return SendResponse(res, res => Ok(res.Heroes?[0]));
        }
     /*   [HttpPost("")]
        public async Task<IActionResult> AddHero([FromBody] AddModel add)
        {
            bool res =await _heroRepository.CreateHero(add.Name, add.ImagePath);
            return Ok(res);
        }*/
        private IActionResult SendResponse(HeroResponseModel res, Func<HeroResponseModel,IActionResult>? getResponseForStatus200=null)
        {
            switch (res.Status)
            {
                case 401:
                    return Unauthorized(res.ErrorMessage);
                case 404:
                    return NotFound(res.ErrorMessage);
                case 400:
                    return BadRequest(res.ErrorMessage);
                case 200:
                    return getResponseForStatus200 == null ? Ok(res.Heroes) : getResponseForStatus200(res);
                default:
                    throw new Exception("we don't have a case for status "+res.Status);
            }
        }
    }
}
