
using AdmBoots.Application.Menus;
using AdmBoots.Application.Menus.Dto;
using AdmBoots.Domain.Framework.Abstractions;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using AdmBoots.Common;

namespace AdmBoots.Api.Controllers
{	
	/// <summary>
	/// 
	/// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]/[action]")]
    [Authorize(GlobalVars.Permissions)]
    public class MenuController : ControllerBase
	{
        private readonly IMenuService _menuService;
        public MenuController(IMenuService menuService)
        {
            _menuService = menuService;
        }
       
    }
}