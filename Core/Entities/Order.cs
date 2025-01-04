using Core.Constants;

namespace Core.Entities;

public class Order : BaseEntity
{
	public OrderStatus Status { get; set; }
	public string UserId { get; set; }
	public User User { get; set; }
    public Guid PaymentToken { get; set; }
    public ICollection<OrderProduct> OrderProducts { get; set; }
}

