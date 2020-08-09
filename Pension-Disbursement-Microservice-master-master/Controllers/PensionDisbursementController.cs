using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


//change get to post

namespace PensionDisbursement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PensionDisbursementController : ControllerBase
    {
        public IConfiguration configuration;
        /// <summary>
        /// Dependency Injection
        /// </summary>
        /// <param name="iConfig"></param>
        public PensionDisbursementController(IConfiguration iConfig)
        {
            configuration = iConfig;
        }
        static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(PensionDisbursementController));
        /// <summary>
        /// Getting the values from Process Pension Microservice
        /// </summary>
        /// <param name="pension"></param>
        /// <returns>Status Code</returns>
        [HttpPost]
        public int GetDisbursePension(ProcessPensionInput pension)
        {
            _log4net.Info("Pension Amount is Being Validated");
            PensionerDetail pensionerDetail = new PensionerDetail();
            _log4net.Info("[Configuration]:" + configuration);
            ServiceWrapper getPensionerDetail = new ServiceWrapper(configuration);

            _log4net.Info("[getPensionerDetail]:" + getPensionerDetail.ToString());
            pensionerDetail = getPensionerDetail.GetDetailResponse(pension.AadharNumber);
            _log4net.Info("[pensionerDetailDetails]:" + pensionerDetail);

            if (pensionerDetail == null)
                return 21;

            int processPensionStatusCode = 0;
            int bankServiceCharge;
            if (pension.BankType == 1)
                bankServiceCharge = 500;
            else if (pension.BankType == 2)
                bankServiceCharge = 550;
            else
                bankServiceCharge = 0;
            double pensionCalculated;
            pensionCalculated = CalculatePensionLogic(pensionerDetail.SalaryEarned, pensionerDetail.Allowances, bankServiceCharge, pensionerDetail.PensionType);

            if (Convert.ToDouble(pension.PensionAmount) == pensionCalculated)
            {
                processPensionStatusCode = 10;
            }
            else
            {
                processPensionStatusCode = 21;
            }
            return processPensionStatusCode;
            
        }
        /// <summary>
        /// Validating the Pension Amount
        /// </summary>
        /// <param name="salaryEarned"></param>
        /// <param name="allowances"></param>
        /// <param name="charge"></param>
        /// <param name="type"></param>
        /// <returns>validated pension amount</returns>
        private double CalculatePensionLogic(int salaryEarned,int allowances,int charge,PensionType type)
        {
            if (type == PensionType.Self)
            {
                return (salaryEarned * 0.8) + allowances + charge;
            }
            else
            {
                return (salaryEarned * 0.5) + allowances + charge;
            }

        }
        

    }
}
