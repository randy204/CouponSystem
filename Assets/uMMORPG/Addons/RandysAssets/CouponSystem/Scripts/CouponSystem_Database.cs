using SQLite;
using System;
using System.Collections.Generic;

public partial class Database {
    class coupons {
        [PrimaryKey, NotNull] public string code { get; set; }
        [NotNull] public int couponType { get; set; }
        [NotNull] public string itemName { get; set; }
        [NotNull] public int amount { get; set; }
        [NotNull] public int singleUse { get; set; }
        [NotNull] public long expiration { get; set; }
    }
    class coupon_uses {
        [NotNull] public string code { get; set; }
        [NotNull] public string account { get; set; }
        [NotNull] public long dateused { get; set; }
    }

    public void Connect_CouponSystem() {
        connection.CreateTable<coupons>();
        connection.CreateTable<coupon_uses>();
        connection.CreateIndex(nameof(coupon_uses), new[] { "code", "account" }, true);
    }

    public CouponSystem_Coupon CouponSystem_GetCoupon(string code) {
        CouponSystem_Coupon returnVal = new CouponSystem_Coupon();

        coupons coup = connection.FindWithQuery<coupons>("SELECT * FROM coupons WHERE code=?", code);

        if(coup != null)
            returnVal = new CouponSystem_Coupon(coup.code, (CouponSystem_CouponType)coup.couponType, coup.itemName, coup.amount, coup.singleUse, coup.expiration);

        return returnVal;
    }

    public bool CouponSystem_CheckIfUsed(string code, string account) {
        return connection.FindWithQuery<coupon_uses>("SELECT * FROM coupon_uses WHERE code=? AND account=?", code, account) != null;
    }

    public void CouponSystem_RemoveCoupon(string code) {
        connection.Execute("DELETE FROM coupons WHERE code=?", code);
    }

    public void CouponSystem_AddCouponUse(string code, string account, long dateUsed) {
        connection.Execute("INSERT INTO coupon_uses VALUES (?, ?, ?)", code, account, dateUsed);
    }

    //USED FOR EDITOR SCRIPTS
    public void CouponSystem_AddCoupon(string code, CouponSystem_CouponType couponType, string itemName, int amount, int singleUse, long expiration) {
        connection.Execute("INSERT INTO coupons VALUES (?, ?, ?, ?, ?, ?)", code, (int)couponType, itemName, amount, singleUse, expiration);
    }

    //USED FOR EDITOR SCRIPTS
    public void CouponSystem_EditCoupon(string oldCode, string newCode, CouponSystem_CouponType couponType, string itemName, int amount, int singleUse, long expiration) {
        object[] paramArry = new object[7] { newCode, couponType, itemName, amount, singleUse, expiration, oldCode };
        connection.Execute("UPDATE coupons SET code=?, couponType=?, itemName=?, amount=?, singleUse=?, expiration=? WHERE code=?", paramArry);
    }

    //USED FOR EDITOR SCRIPTS
    public void CouponSystem_RemoveCouponUse(string code, string account) {
        connection.Execute("DELETE FROM coupon_uses WHERE code=@code AND account=?", code, account);
    }

    //USED FOR EDITOR SCRIPTS
    public List<CouponSystem_Coupon> CouponSystem_GetCoupons() {
        List<CouponSystem_Coupon> coupList = new List<CouponSystem_Coupon>();

        foreach(coupons row in connection.Query<coupons>("SELECT * FROM coupons")) {
            coupList.Add(new CouponSystem_Coupon(row.code, (CouponSystem_CouponType)row.couponType, row.itemName, row.amount, row.singleUse, row.expiration));
        }

        return coupList;
    }

    //USED FOR EDITOR SCRIPTS
    public List<CouponSystem_CouponUse> CouponSystem_GetCouponUses() {
        List<CouponSystem_CouponUse> coupUsesList = new List<CouponSystem_CouponUse>();

        foreach(coupon_uses row in connection.Query<coupon_uses>("SELECT * FROM coupon_uses")) {
            coupUsesList.Add(new CouponSystem_CouponUse(row.code, row.account, row.dateused));
        }

        return coupUsesList;
    }
}