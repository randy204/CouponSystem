using Mirror;
using System;

public partial class Player {
    public void CouponSystem_ParseCoupon(string coupon) {
#if RCS2D3D || RCSCE
        CouponSystem_CouponResult returnVal = CouponSystem_Validate(coupon);

        if(returnVal.error == 0) {
            if(returnVal.couponType == CouponSystem_CouponType.Coins) {
#if RCSCE
                itemMall.coins += returnVal.amount;
#elif RCS2D3D
                coins += returnVal.amount;
#endif
            } else if(returnVal.couponType == CouponSystem_CouponType.Gold) {
                gold += returnVal.amount;
            } else if(returnVal.couponType == CouponSystem_CouponType.Item) {
                ScriptableItem itemData;

                if(ScriptableItem.dict.TryGetValue(returnVal.itemName.GetStableHashCode(), out itemData)) {
#if RCSCE
                    inventory.Add(new Item(itemData), returnVal.amount);
#elif RCS2D3D
                    InventoryAdd(new Item(itemData), returnVal.amount);
#endif
                }
            }
        }

        string reward = "";
        
        if(returnVal.couponType == CouponSystem_CouponType.Coins) reward = CouponSystem_CouponType.Coins.ToString();
        else if(returnVal.couponType == CouponSystem_CouponType.Gold) reward = CouponSystem_CouponType.Gold.ToString();
        else reward = returnVal.itemName;

        Target_CouponSystem_Message(connectionToClient, returnVal.error, reward, returnVal.amount);
#endif
    }

    private CouponSystem_CouponResult CouponSystem_Validate(string code) {
        CouponSystem_Coupon coupon = Database.singleton.CouponSystem_GetCoupon(code);

        if(!coupon.code.Equals("")) {
            if(GetEpochTime() < coupon.expiration) {
                bool used = Database.singleton.CouponSystem_CheckIfUsed(code, account);
                bool canProcess = true;

                if(!used) {
                    if(coupon.couponType == CouponSystem_CouponType.Item) {
                        ScriptableItem itemData;

                        if(ScriptableItem.dict.TryGetValue(coupon.itemName.GetStableHashCode(), out itemData)) {
                            Item item = new Item(itemData);

#if RCSCE
	                        if(!inventory.CanAdd(item, coupon.amount))
                                canProcess = false;
#elif RCS2D3D
                            if(!InventoryCanAdd(item, coupon.amount))
                                canProcess = false;
#endif

                        }
                    }
                    
                    if(canProcess) {
                        if(coupon.singleUse == 1) {
                            Database.singleton.CouponSystem_RemoveCoupon(coupon.code);
                        }

                        Database.singleton.CouponSystem_AddCouponUse(coupon.code, account, GetEpochTime());

                        return new CouponSystem_CouponResult(coupon.couponType, coupon.itemName, coupon.amount);
                    } else {
                        return new CouponSystem_CouponResult(4);
                    }
                } else {
                    return new CouponSystem_CouponResult(1);
                }
            } else {
                return new CouponSystem_CouponResult(2);
            }
        } else {
            return new CouponSystem_CouponResult(3);
        }
    }

    public static long GetEpochTime() {
        return (long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
    }

    [TargetRpc]
    public void Target_CouponSystem_Message(NetworkConnection target, int messageID, string reward, int amount) {
        string message = CouponSystem_ResultMsgs.messages[messageID];

        if(!string.IsNullOrWhiteSpace(reward)) {
            message += " You have claimed: " + amount + " " + reward + ".";
        }

#if RCSCE
	    UIChat.singleton.AddMessage(new ChatMessage("", chat.infoChannel.identifierIn, message, "", chat.infoChannel.textPrefab));
#elif RCS2D3D
        UIChat.singleton.AddMessage(new ChatMessage("", chat.info.identifierIn, message, "", chat.info.textPrefab));
#endif
    }
}