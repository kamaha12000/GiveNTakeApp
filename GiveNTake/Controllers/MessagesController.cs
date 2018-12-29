using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace GiveNTake.Controllers
{
    public class MessagesController : Controller
    {
        public string[] My()
        {
            return new[]
            {
                "Is the Microwave working", "Where can I pick the washing machine from?"
            };
        }

        public string Details(int id)
        {
            return $"{id} - Is the Microwave working?";
        }
    }
}
