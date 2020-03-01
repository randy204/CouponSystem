public class CouponSystem_CouponUse {
    public string code;
    public string account;
    public long dateused;

    public CouponSystem_CouponUse() {
        code = "";
    }

    public CouponSystem_CouponUse(string code, string account, long dateused) {
        this.code = code;
        this.account = account;
        this.dateused = dateused;
    }
}