# TicketPlus 遠大售票自動訂票機器人

藉由遠大售票系統探討實作自動化訂票機器人的流程。

[https://ticketplus.com.tw/](https://ticketplus.com.tw/)

# 使用說明

純後端開發，無友善的UI呈現，僅提供swagger & redoc 呈現 API 的 schema。

![要享受這個過程](./img/api.jpg)

> POST /api/v1/ticketplus/autoreserve 

可以實現自動化訂票流程，相關運行情況請參考log。

---

> PUT /api/v1/ticketplus/user/cache  
> PUT /api/v1/ticketplus/activity/cache

可事前將場次資訊與使用者資訊儲存(YOASOBI場次有遇到順發流量導致s3資訊與登入api出現異常，但後來基本上沒遇到過)

---

> GET /api/v1/ticketplus/s3productinfo  
> GET /api/v1/ticketplus/productconfig  
> GET /api/v1/ticketplus/areaconfig  
> GET /api/v1/ticketplus/accesstoken

各種場次，票券，登入相關的API，沒意外不會特別用到

---

> POST /api/v1/ticketplus/captcha  
> POST /api/v1/ticketplus/captcha/parsing  

取得驗證碼與解析的API

---

> POST /api/v1/ticketplus/reserve  

實際訂票的API

# 執行方法

將專案 pull 後直接 run 就可以執行，電腦環境需具備.net 7以上的sdk & runtime。

僅測試過windows平台可使用，但理論上linux / docker 環境都能正常運行。

# 技術探討 (TL;DR)

## 啟發

透過chrome的開發者工具觀察network，可以觀察出遠大售票使用相當完整的前後端分離，透過前端的vue與後端的nodejs api(?)搭建而成，進而推斷出或許可以使用純api call的方式來完成訂票流程。

進而觀察後，在訂票流程最關鍵的搶票過程後，剩下會有充裕的時間可以完成後續的流程(付款，實名制)，且可以透過官網的 會員專區-我的訂單 找到成功搶到票的資訊，並接續完成後續流程。

所以只要完成前面訂票的流程自動化，剩下的可以在網頁上操作補完。

## 需求分析

### 從API觀察，完成售票流程需要的資訊會有：

1. Product: 主要的票券資訊，包含 productId(票券Id), ticketAreaId(區域Id), name(票種名稱 Ex. VIP票, 一般票)。來自於 https://apis.ticketplus.com.tw/config/api/v1/getS3?path=event/54fdee72c71c18cc2b694801e11e77cd/products.json ，54fdee72c71c18cc2b694801e11e77cd 為 場次ID(ActivityId)。

2. ProductConfig: 票券與相關設定，主要是要其中的 sessionId，用於驗證碼的取得與預約票券。來自於 https://apis.ticketplus.com.tw/config/api/v1/get?productId=p000002989 ，p000002989 為 票券Id(productId)。

3. TicketAreaConfig: 票區相關設定，像是 2F座席F區 這種名稱的資訊會在裡面。不一定每個場次都會有，有些會使用name(票種名稱)來簡單使用，就不會有票區資訊。來自於 https://apis.ticketplus.com.tw/config/api/v1/get?ticketAreaId=a000000887 ，a000000887 為 票區Id(ticketAreaId)

4. UserInfo: 使用者相關資訊，主要是需要 accessToken 來取得驗證碼跟訂票。來自於 https://apis.ticketplus.com.tw/user/api/v1/login 。

5. Captcha: 遠大自行定義的驗證方式，透過每個場次會有獨特的sessionId與會員ID，定義出這個場次/會員這次驗證碼的答案。來自於 https://apis.ticketplus.com.tw/captcha/api/v1/generate ，會回傳svg的data表示驗證碼的圖案。

6. Reserve: 將以上獲得的資訊：productId(票券Id)，購買張數，Captcha，accessToken，組合成這次預約票券需要的資訊。

### 特有的一些邏輯

1. 預約票券的邏輯類似於Queue，必須在購買的票券還有票 且 驗證碼正確 且 沒有任何預約在Queue 的情況下，才能成功地進去Queue裡等待，這個情形下，預約票券的API會回傳pending，需要重複呼叫API，才能知道預約成功 or 失敗。(沒錯，進去Queue不代表成功訂到票)

2. 承上，在Queue的先後順序不代表購票順序，推測會在一段時間內(15~30秒)開放購票者進去，並隨機選擇成功購票的人，剩餘的會被踢出Queue。

3. Captcha的邏輯推估有點像token桶，只有在開賣期間才能把這次Captcha的答案放進去，並在預約票券的時候進行比對，還沒開賣期間不會放進去，所以藉由Captcha比對失敗來限制購買。同時，只要沒有成功的進入預約票券的Queue，Captcha的答案是不會變的。

4. 密碼是用MD5加密。

### 預約自動化邏輯

綜合以上資訊，只要透過場次Id(ActivityId)與會員手機與密碼，就可以透過API取得其他相關資訊完成訂票。

## 程式設計架構

採用DDD設計，結合MediatR與CQRS的架構，WebAPI作為程式的呼叫點。

使用 FluentValidation 進行參數的驗證。

使用xUnit撰寫單元測試。

# 學術研究聲明

本專案僅用於學術研究。  
請支持手動搶票，等待的過程與忐忑的心也是搶票的醍醐味，困難的道路才有豐碩的果實。

![要享受這個過程](./img/1658228256824.gif)

# 關於驗證碼

目前使用自動辨識驗證碼用的元件是:

https://github.com/sml2h3/ddddocr  
https://github.com/zixing131/ddddocrsharp