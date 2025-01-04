using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services.Abstract;

public interface IPaymentService
{
	Task<(int statusCode, string? description, string? id)> PayAsync();
	Task<bool> PaySuccess(Guid token);
	Task<bool> PayCancel(Guid token);
}
