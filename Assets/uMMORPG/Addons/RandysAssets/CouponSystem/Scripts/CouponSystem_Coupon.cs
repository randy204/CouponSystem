public class CouponSystem_Coupon {
    public string code;
    public CouponSystem_CouponType couponType;
    public string itemName;
    public int amount;
    public int singleUse;
    public long expiration;

    public CouponSystem_Coupon() {
        code = "";
    }

    public CouponSystem_Coupon(string code, CouponSystem_CouponType couponType, string itemName, int amount, int singleUse, long expiration) {
        this.code = code;
        this.couponType = couponType;
        this.itemName = itemName;
        this.amount = amount;
        this.singleUse = singleUse;
        this.expiration = expiration;
    }
}