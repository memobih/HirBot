using Project.Repository.Repository;
using Project.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exame.Services.Implemntation
{
    public class QuestionService
    {

        private readonly IAuthenticationService _authenticationService;
        private readonly UnitOfWork _unitOfWork;
        public QuestionService(IAuthenticationService authenticationService, UnitOfWork unitOfWork)
        {
            _authenticationService = authenticationService;

            _unitOfWork = unitOfWork;
        }

    }
}
