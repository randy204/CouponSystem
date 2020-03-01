/// <summary>
/// Error Codes:
/// (1: Coupon allready redeemed on account)
/// (2: Coupon has expired)
/// (3: Coupon is invalid)
/// (4: Inventory is full)
/// </summary>
public class CouponSystem_CouponResult {
    public CouponSystem_CouponType couponType = CouponSystem_CouponType.Item;
    public string itemName = "";
    public int amount = 0;
    public int error = 0;

    public CouponSystem_CouponResult(CouponSystem_CouponType couponType, string itemName, int amount) {
        this.couponType = couponType;
        this.itemName = itemName;
        this.amount = amount;
    }

    public CouponSystem_CouponResult(int error) {
        this.error = error;
    }
}