namespace Axiom.Common
{
    using System;
    using System.Collections.Generic;

    #region Common

    /// <summary>
    /// This class will use during communication with API to get response
    /// </summary>
    public class BaseApiResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseApiResponse" /> class.
        /// </summary>
        public BaseApiResponse()
        {
            this.Message = new List<string> { };
        }

        /// <summary>
        /// Gets or sets a value indicating whether response is success or fail
        /// </summary>
        /// <value>
        ///   <c>true</c> if success; otherwise, <c>false</c>.
        /// </value>
        public bool Success { get; set; }

        /// <summary>
        /// Gets or sets Message
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        /// 


        public int? InsertedId { get; set; }
        public int? ArID { get; set; }
        public long? lng_InsertedId { get; set; }
        public string str_ResponseData{ get; set; }
        public IList<string> Message { get; set; }
    }

    /// <summary>
    /// This class will use to transfer data during communication with API
    /// </summary>
    /// <typeparam name="T">Entity class object</typeparam>
    public class ApiResponse<T> : BaseApiResponse
    {
        /// <summary>
        /// Gets or sets list of data
        /// </summary>
        /// <value>
        /// The data.
        /// </value>
        public virtual IList<T> Data { get; set; }
    }

    /// <summary>
    /// This class will use to get response from insert/update operation of Provider
    /// </summary>
    public class ApiMissingTitleResult : BaseApiResponse
    {
        /// <summary>
        /// Gets or sets the provider identifier.
        /// </summary>
        /// <value>
        /// The provider identifier.
        /// </value>
        public long ProviderID { get; set; }

        /// <summary>
        /// Gets or sets TitleID
        /// </summary>
        /// <value>
        /// The title identifier.
        /// </value>
        public long TitleID { get; set; }
    }


    /// <summary>
    /// This class will use get response
    /// </summary>
    /// <typeparam name="T">Entity class object</typeparam>
    public class ApiPostResponse<T> : BaseApiResponse
    {
        /// <summary>
        /// Gets or sets data
        /// </summary>
        /// <value>
        /// The data.
        /// </value>
        public virtual T Data { get; set; }
    }

    #endregion

    #region Individual

    /// <summary>
    /// This class will use to get response from insert/update operation of Contract
    /// </summary>
    public class ApiContractCreateResult : BaseApiResponse
    {
        /// <summary>
        /// Gets or sets ContractId
        /// </summary>
        /// <value>
        /// The contract identifier.
        /// </value>
        public int ContractID { get; set; }
    }

    /// <summary>
    /// This class will use to get response from insert/update operation of Contract
    /// </summary>
    public class ApiServiceAddendumAddUpdate : BaseApiResponse
    {
        /// <summary>
        /// Gets or sets Addendum
        /// </summary>
        /// <value>
        /// The Addendum identifier.
        /// </value>
        public int AddendumID { get; set; }
    }

    /// <summary>
    /// This class will use to get response from insert/update operation of Asset Requirement
    /// </summary>
    public class AssetRequirementApiCreateResultWithId : BaseApiResponse
    {
        /// <summary>
        /// Gets or sets AssetReqId
        /// </summary>
        /// <value>
        /// The asset requirement identifier.
        /// </value>
        public long AssetReqID { get; set; }
    }

    /// <summary>
    /// This class will use to get response from insert/update operation of Asset Type
    /// </summary>
    public class AssetTypeApiCreateResultWithId : BaseApiResponse
    {
        /// <summary>
        /// Gets or sets AssetTypeId
        /// </summary>
        /// <value>
        /// The asset type identifier.
        /// </value>
        public int AssetTypeID { get; set; }
    }

    /// <summary>
    /// This class will use to get response from insert/update/select operation of Title Type
    /// </summary>
    public class ApiTitleTypeCreateResultWithId : BaseApiResponse
    {
        /// <summary>
        /// Gets or sets TTID
        /// </summary>
        /// <value>
        /// The ttid.
        /// </value>
        public long TTID { get; set; }
    }

    /// <summary>
    /// This class will use to get response from insert/update operation of Service Field
    /// </summary>
    public class ApiServiceFieldsCreateResult : BaseApiResponse
    {
        /// <summary>
        /// Gets or sets SFID
        /// </summary>
        /// <value>
        /// The sfid.
        /// </value>
        public long SFID { get; set; }
    }

    /// <summary>
    /// This class will use to get response from insert/update operation of Title
    /// </summary>
    public class ApiTitleCreateResult : BaseApiResponse
    {
        /// <summary>
        /// Gets or sets TitleID
        /// </summary>
        /// <value>
        /// The title identifier.
        /// </value>
        public long TitleID { get; set; }
    }

    /// <summary>
    /// This class will use to get response from insert/update operation of Master field list (Title field list)
    /// </summary>
    public class ApiMasterFieldCreateFieldResult : BaseApiResponse
    {
        /// <summary>
        /// Gets or sets FieldID
        /// </summary>
        /// <value>
        /// The field identifier.
        /// </value>
        public long FieldID { get; set; }
    }

    /// <summary>
    /// This class will use to get response from insert/update operation of Field control value
    /// </summary>
    public class ApiFieldControlValueCreateFieldResult : BaseApiResponse
    {
        /// <summary>
        /// Gets or sets FCVID
        /// </summary>
        /// <value>
        /// The fcvid.
        /// </value>
        public long FCVID { get; set; }
    }

    /// <summary>
    /// This class will use to get response from insert/update operation of Reorder Fields
    /// </summary>
    public class ApiReorderFieldsResult : BaseApiResponse
    {
        /// <summary>
        /// Gets or sets FieldOrder
        /// </summary>
        /// <value>
        /// The field order.
        /// </value>
        public int FieldOrder { get; set; }
    }

    /// <summary>
    /// This class will use to get response from insert/update operation of Value Priority
    /// </summary>
    public class ApiReorderTaValuePrioritiesResult : BaseApiResponse
    {
        /// <summary>
        /// Gets or sets Value Priority
        /// </summary>
        /// <value>
        /// The value priority.
        /// </value>
        public int ValuePriority { get; set; }
    }

    /// <summary>
    /// This class will use to get response from insert/update operation of Priority
    /// </summary>
    public class ApiReorderAssetRequirementResult : BaseApiResponse
    {
        /// <summary>
        /// Gets or sets Priority
        /// </summary>
        /// <value>
        /// The priority.
        /// </value>
        public long Priority { get; set; }
    }

    /// <summary>
    /// This class will use to get response from insert/update operation of Service Language Pref
    /// </summary>
    public class ApiServiceLangPrefsCreateResult : BaseApiResponse
    {
        /// <summary>
        /// Gets or sets SLPID
        /// </summary>
        /// <value>
        /// The slpid.
        /// </value>
        public long SLPID { get; set; }
    }

    /// <summary>
    /// This class will use to get response from insert/update operation of Specification
    /// </summary>
    public class ApiSpecificationCreateResult : BaseApiResponse
    {
        /// <summary>
        /// Gets or set SpecID
        /// </summary>
        /// <value>
        /// The spec identifier.
        /// </value>
        public int SpecID { get; set; }
    }

    /// <summary>
    /// This class will use to get response from insert/update operation of Title Attribute
    /// </summary>
    public class ApiTitleAttributeCreateResultWithId : BaseApiResponse
    {
        /// <summary>
        /// Gets or sets TAID
        /// </summary>
        /// <value>
        /// The taid.
        /// </value>
        public long TAID { get; set; }
    }

    /// <summary>
    /// This class will use to get response from update operation of Title
    /// </summary>
    public class ApiUpdateTitle : BaseApiResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApiUpdateTitle" /> class.
        /// </summary>
        public ApiUpdateTitle()
        {
            this.TAIDs = new List<long> { };
        }

        /// <summary>
        /// Gets or sets TAIDS
        /// </summary>
        /// <value>
        /// The tai ds.
        /// </value>
        public IList<long> TAIDs { get; set; }
    }

    /// <summary>
    /// This class will use to get response from insert/update operation of Language
    /// </summary>
    public class ApiTitleAttributeCreateLanguage : BaseApiResponse
    {
        /// <summary>
        /// Gets or sets LanguageID
        /// </summary>
        /// <value>
        /// The language identifier.
        /// </value>
        public long LanguageID { get; set; }
    }

    /// <summary>
    /// This class will use to get response from insert/update operation of Language
    /// </summary>
    public class ApiCreatePricingRangeItem : BaseApiResponse
    {
        /// <summary>
        /// Gets or sets Pricing Range ID
        /// </summary>
        /// <value>
        /// The Pricing Range identifier.
        /// </value>
        public int PricingRangeID { get; set; }
    }

    /// <summary>
    /// This class will use to get response from insert/update operation of Currency
    /// </summary>
    public class ApiCreateCurrency : BaseApiResponse
    {
        /// <summary>
        /// Gets or sets CurrencyID
        /// </summary>
        /// <value>
        /// The currency identifier.
        /// </value>
        public long CurrencyID { get; set; }
    }

    /// <summary>
    /// This class will use to get response from insert/update operation of Contract Description
    /// </summary>
    public class ContractsDescription : BaseApiResponse
    {
        /// <summary>
        /// Gets or set ContractDescription
        /// </summary>
        /// <value>
        /// The contract description.
        /// </value>
        public string ContractDescription { get; set; }

        /// <summary>
        /// Gets or sets UploadFile
        /// </summary>
        /// <value>
        /// The uploaded file.
        /// </value>
        public string UploadedFile { get; set; }
    }

    /// <summary>
    /// This class will use to get response for preview Addendum Text
    /// </summary>
    public class ServiceAddendumDescription : BaseApiResponse
    {
        /// <summary>
        /// Gets or set ContractDescription
        /// </summary>
        /// <value>
        /// The contract description.
        /// </value>
        public string AddendumText { get; set; }
    }

    /// <summary>
    /// This class will use to get response from insert/update operation of File name
    /// </summary>
    public class GetFileName : BaseApiResponse
    {
        /// <summary>
        /// Gets or sets the name of the file.
        /// </summary>
        /// <value>
        /// The name of the file.
        /// </value>
        public string FileName { get; set; }
    }

    public class AddEditTerritoryRequirement : BaseApiResponse
    {
        /// <summary>
        /// Gets or sets the name of the file.
        /// </summary>
        /// <value>
        /// The name of the file.
        /// </value>
        public long TerritoryRequirementID { get; set; }
    }

    /// <summary>
    /// This class will use to get response from insert/update operation of Territory from order
    /// </summary>
    public class RemoveThisTerritoryFromOrder : BaseApiResponse
    {
        /// <summary>
        /// Gets or sets the name of the territory.
        /// </summary>
        /// <value>
        /// The name of the territory.
        /// </value>
        public string TerritoryName { get; set; }

        /// <summary>
        /// Gets or sets the name of the continent.
        /// </summary>
        /// <value>
        /// The name of the continent.
        /// </value>
        public string ContinentName { get; set; }

        /// <summary>
        /// Gets or sets the language.
        /// </summary>
        /// <value>
        /// The language.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the is language required.
        /// </summary>
        /// <value>
        /// The is language required.
        /// </value>
        public bool? LanguageRequired { get; set; }
    }

    /// <summary>
    /// This class will use to get response from insert/update operation of Requirement Task
    /// </summary>
    public class ApiCreateRequirementTask : BaseApiResponse
    {
        /// <summary>
        /// Gets or sets the task identifier.
        /// </summary>
        /// <value>
        /// The task identifier.
        /// </value>
        public int TaskID { get; set; }
    }

    /// <summary>
    /// This class will use to get response from insert/update operation of Requirement Status
    /// </summary>
    public class ApiCreateRequirementStatus : BaseApiResponse
    {
        /// <summary>
        /// Gets or sets the status identifier.
        /// </summary>
        /// <value>
        /// The status identifier.
        /// </value>
        public int StatusID { get; set; }
    }

    /// <summary>
    /// This class will use to get response from select operation of Service Field
    /// </summary>
    public class ApiFieldsForServiceResult
    {
        /// <summary>
        /// Gets or sets the field identifier.
        /// </summary>
        /// <value>
        /// The field identifier.
        /// </value>
        public long FieldID { get; set; }

        /// <summary>
        /// Gets or sets the name of the field.
        /// </summary>
        /// <value>
        /// The name of the field.
        /// </value>
        public string FieldName { get; set; }

        /// <summary>
        /// Gets or sets the field description.
        /// </summary>
        /// <value>
        /// The field description.
        /// </value>
        public string FieldDescription { get; set; }

        /// <summary>
        /// Gets or sets the is language specific.
        /// </summary>
        /// <value>
        /// The is language specific.
        /// </value>
        public bool? IsLanguageSpecific { get; set; }

        /// <summary>
        /// Gets or sets the required.
        /// </summary>
        /// <value>
        /// The required.
        /// </value>
        public int? Required { get; set; }

        /// <summary>
        /// Gets or sets the service identifier.
        /// </summary>
        /// <value>
        /// The service identifier.
        /// </value>
        public int ServiceID { get; set; }

        /// <summary>
        /// Gets or sets the sfid.
        /// </summary>
        /// <value>
        /// The sfid.
        /// </value>
        public long SFID { get; set; }

        /// <summary>
        /// Gets or sets the territory code.
        /// </summary>
        /// <value>
        /// The territory code.
        /// </value>
        public string TerritoryCode { get; set; }

        /// <summary>
        /// Gets or sets the ttid.
        /// </summary>
        /// <value>
        /// The ttid.
        /// </value>
        public int? TTID { get; set; }

        /// <summary>
        /// Gets or sets the type of the title.
        /// </summary>
        /// <value>
        /// The type of the title.
        /// </value>
        public string TitleType { get; set; }

        /// <summary>
        /// Gets or sets the vod required.
        /// </summary>
        /// <value>
        /// The vod required.
        /// </value>
        public bool? VODRequired { get; set; }

        /// <summary>
        /// Gets or sets the est required.
        /// </summary>
        /// <value>
        /// The est required.
        /// </value>
        public bool? ESTRequired { get; set; }

        /// <summary>
        /// Gets or sets the needs review.
        /// </summary>
        /// <value>
        /// The needs review.
        /// </value>
        public bool? NeedsReview { get; set; }

        /// <summary>
        /// Gets or sets the localization.
        /// </summary>
        /// <value>
        /// The localization.
        /// </value>
        public int? Localization { get; set; }

        /// <summary>
        /// Gets or sets the allow localization by territory.
        /// </summary>
        /// <value>The allow localization by territory.</value>        
        public bool? AllowLocalizationbyTerritory { get; set; }

        /// <summary>
        /// Gets or sets the territory.
        /// </summary>
        /// <value>
        /// The territory.
        /// </value>
        public string Territory { get; set; }

        /// <summary>
        /// Gets or sets the site identifier.
        /// </summary>
        /// <value>
        /// The site identifier.
        /// </value>
        public int[] SiteId { get; set; }
    }

    /// <summary>
    /// This class will use to get response from insert/update operation of Order
    /// </summary>
    public class ApiCreateOrder : BaseApiResponse
    {
        /// <summary>
        /// Gets or sets the order identifier.
        /// </summary>
        /// <value>
        /// The order identifier.
        /// </value>
        public long OrderID { get; set; }
    }

    /// <summary>
    /// This class will use to get response from insert/update operation of Order Contract
    /// </summary>
    public class ApiUpdateOrderContract : BaseApiResponse
    {
        /// <summary>
        /// Gets or sets the order identifier.
        /// </summary>
        /// <value>
        /// The order identifier.
        /// </value>
        public long OrderID { get; set; }
    }

    /// <summary>
    /// This class will use to get response from insert/update operation of Order Language
    /// </summary>
    public class APIReOrder_Language : BaseApiResponse
    {
        /// <summary>
        /// Gets or sets the display order.
        /// </summary>
        /// <value>
        /// The display order.
        /// </value>
        public int DisplayOrder { get; set; }
    }

    /// <summary>
    /// This class will use to get response from insert/update operation of Payment Plan
    /// </summary>
    public class ApiPaymentPlanAddEditResult : BaseApiResponse
    {
        /// <summary>
        /// Gets or sets the plan identifier.
        /// </summary>
        /// <value>
        /// The plan identifier.
        /// </value>
        public int PlanID { get; set; }
    }

    /// <summary>
    /// This class will use to get response from insert/update operation of Order Review Details
    /// </summary>
    public class GetOrderReviewResult : BaseApiResponse
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public int ID { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the qty.
        /// </summary>
        /// <value>
        /// The qty.
        /// </value>
        public int? Qty { get; set; }

        /// <summary>
        /// Gets or sets the price.
        /// </summary>
        /// <value>
        /// The price.
        /// </value>
        public decimal? Price { get; set; }

        /// <summary>
        /// Gets or sets the admin price.
        /// </summary>
        /// <value>
        /// The admin price.
        /// </value>
        public decimal? AdminPrice { get; set; }

        /// <summary>
        /// Gets or sets the parent identifier.
        /// </summary>
        /// <value>
        /// The parent identifier.
        /// </value>
        public int? ParentID { get; set; }

        /// <summary>
        /// Gets or sets the plan description.
        /// </summary>
        /// <value>
        /// The plan description.
        /// </value>
        public string PlanDescription { get; set; }

        /// <summary>
        /// Gets or sets the order cost identifier.
        /// </summary>
        /// <value>
        /// The order cost identifier.
        /// </value>
        public long? OrderCostId { get; set; }

        /// <summary>
        /// Gets or sets the manual override cost.
        /// </summary>
        /// <value>
        /// The manual override cost.
        /// </value>
        public decimal? ManualOverrideCost { get; set; }

        /// <summary>
        /// Gets or sets the manual override flag.
        /// </summary>
        /// <value>
        /// The manual override flag.
        /// </value>
        public bool? ManualOverrideFlag { get; set; }

        /// <summary>
        /// Gets or sets the indent level.
        /// </summary>
        /// <value>
        /// The indent level.
        /// </value>
        public int IndentLevel { get; set; }

        /// <summary>
        /// Gets or sets the total.
        /// </summary>
        /// <value>
        /// The total.
        /// </value>
        public string Total { get; set; }

        /// <summary>
        /// Gets or sets the Discount Amount.
        /// </summary>
        /// <value>
        /// The DiscountAmount.
        /// </value>
        public decimal? DiscountAmount { get; set; }

        /// <summary>
        /// Gets or sets the discount message.
        /// </summary>
        /// <value>
        /// The discount message.
        /// </value>
        public string DiscountMessage { get; set; }
    }

    ///// <summary>
    ///// This class will use to get response from select operation of services
    ///// </summary>
    //public class ApiServicesResult
    //{
    //    /// <summary>
    //    /// Gets or sets the service identifier.
    //    /// </summary>
    //    /// <value>
    //    /// The service identifier.
    //    /// </value>
    //    public int ServiceID { get; set; }
    //    /// <summary>
    //    /// Gets or sets the name.
    //    /// </summary>
    //    /// <value>
    //    /// The name.
    //    /// </value>
    //    public string Name { get; set; }
    //    /// <summary>
    //    /// Gets or sets the notes.
    //    /// </summary>
    //    /// <value>
    //    /// The notes.
    //    /// </value>
    //    public string Notes { get; set; }
    //}

    /// <summary>
    /// This class will use to get response from insert/update operation of Pricing
    /// </summary>
    public class ApiAddPricing : BaseApiResponse
    {
        /// <summary>
        /// Gets or sets the billable item identifier.
        /// </summary>
        /// <value>
        /// The billable item identifier.
        /// </value>
        public int BillableItemID { get; set; }
    }

    /// <summary>
    /// This class will use to get response from insert/update operation of Order Pricing
    /// </summary>
    public class ApiReOrderPricing : BaseApiResponse
    {
        /// <summary>
        /// Gets or sets the sort order.
        /// </summary>
        /// <value>
        /// The sort order.
        /// </value>
        public int SortOrder { get; set; }
    }

    /// <summary>
    /// This class will use to get response from insert/update operation of Provider
    /// </summary>
    public class ApiProviderCreateResult : BaseApiResponse
    {
        /// <summary>
        /// Gets or sets the provider identifier.
        /// </summary>
        /// <value>
        /// The provider identifier.
        /// </value>
        public long ProviderID { get; set; }
    }

    /// <summary>
    /// This class will use to get response from insert/update operation of Site Setting
    /// </summary>
    public class ApiSiteSettingCreateResult : BaseApiResponse
    {
        /// <summary>
        /// Gets or sets the site setting identifier.
        /// </summary>
        /// <value>The site setting identifier.</value>
        public long SiteSettingId { get; set; }
    }

    /// <summary>
    /// This class will use to get response from insert/update operation of Services
    /// </summary>
    public class ApiServiceCreateResult : BaseApiResponse
    {
        /// <summary>
        /// Gets or sets the service identifier.
        /// </summary>
        /// <value>
        /// The service identifier.
        /// </value>
        public long ServiceID { get; set; }
    }

    /// <summary>
    /// This class will use to get response from insert/update operation of Alert Messages
    /// </summary>
    public class ApiCreateAlertMessage : BaseApiResponse
    {
        /// <summary>
        /// Gets or sets the alert identifier.
        /// </summary>
        /// <value>
        /// The alert identifier.
        /// </value>
        public long AlertID { get; set; }
    }

    /// <summary>
    /// This class will use to get response from insert/update operation of Alert Type
    /// </summary>
    public class ApiCreateAlertType : BaseApiResponse
    {
        /// <summary>
        /// Gets or sets the type identifier.
        /// </summary>
        /// <value>
        /// The type identifier.
        /// </value>
        public int TypeID { get; set; }
    }

    /// <summary>
    /// This class will use to get response from insert/update operation of Finance Details
    /// </summary>
    public class ApiFinanceDetails : BaseApiResponse
    {
        /// <summary>
        /// Gets or sets the provider identifier.
        /// </summary>
        /// <value>
        /// The provider identifier.
        /// </value>
        public long? ProviderId { get; set; }

        /// <summary>
        /// Gets or sets the name of the service.
        /// </summary>
        /// <value>
        /// The name of the service.
        /// </value>
        public string ServiceName { get; set; }

        /// <summary>
        /// Gets or sets the title identifier.
        /// </summary>
        /// <value>
        /// The title identifier.
        /// </value>
        public long? TitleId { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the date.
        /// </summary>
        /// <value>
        /// The date.
        /// </value>
        public DateTime Date { get; set; }

        /// <summary>
        /// Gets or sets the price.
        /// </summary>
        /// <value>
        /// The price.
        /// </value>
        public decimal? Price { get; set; }

        /// <summary>
        /// Gets or sets the video format.
        /// </summary>
        /// <value>
        /// The video format.
        /// </value>
        public int? VideoFormat { get; set; }

        /// <summary>
        /// Gets or sets the total count.
        /// </summary>
        /// <value>
        /// The total count.
        /// </value>
        public int TotalCount { get; set; }

        /// <summary>
        /// Country Code
        /// </summary>
        public string CountryCode { get; set; }

        /// <summary>
        /// Customer Currency
        /// </summary>
        public string CustomerCurrency { get; set; }

        /// <summary>
        /// Customer ID
        /// </summary>
        public string CustomerID { get; set; }

        /// <summary>
        /// Customer Price
        /// </summary>
        public decimal? CustomerPrice { get; set; }

        /// <summary>
        /// Order ID
        /// </summary>
        public string OrderID { get; set; }

        /// <summary>
        /// Parent ID
        /// </summary>
        public string ParentID { get; set; }

        /// <summary>
        /// Platform Reference ID
        /// </summary>
        public string PlatformReferenceID { get; set; }

        /// <summary>
        /// Provider
        /// </summary>
        public string Provider { get; set; }

        /// <summary>
        /// Provider Title Id
        /// </summary>
        public string ProviderTitleId { get; set; }

        /// <summary>
        /// Quantity
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Royalty Currency
        /// </summary>
        public string RoyaltyCurrency { get; set; }

        /// <summary>
        /// Service ID
        /// </summary>
        public int ServiceID { get; set; }

        /// <summary>
        /// Transaction Type
        /// </summary>
        public int TransactionType { get; set; }

        /// <summary>
        /// Vendor ID
        /// </summary>
        public string VendorID { get; set; }

        /// <summary>
        /// Sales Type
        /// </summary>
        public string SalesType { get; set; }

        /// <summary>
        /// Postal Code
        /// </summary>
        public string PostalCode { get; set; }

        /// <summary>
        /// Product Type
        /// </summary>
        public string ProductType { get; set; }

        /// <summary>
        /// Gets or sets the Total Quantity.
        /// </summary>
        /// <value>The Total Quantity.</value>
        public long TotalQuantity { get; set; }

        /// <summary>
        /// Gets or sets the Total Price.
        /// </summary>
        /// <value>The Total Price.</value>
        public decimal? TotalPrice { get; set; }

        /// <summary>
        /// Gets or sets The Native Final Partner Earning
        /// </summary>
        public string NativeFinalPartnerEarning { get; set; }

        /// <summary>
        /// Gets or sets The Type
        /// </summary>
        public string Type { get; set; }

    }

    ///// <summary>
    ///// This class will use to get response from insert/update operation of Asset
    ///// </summary>
    //public class ApiAssetCreateResult : BaseApiResponse
    //{
    //    /// <summary>
    //    /// Gets or sets the asset identifier.
    //    /// </summary>
    //    /// <value>
    //    /// The asset identifier.
    //    /// </value>
    //    public long AssetID { get; set; }
    //}

    /// <summary>
    /// This class will use to get response from insert/update operation of Service Setup
    /// </summary>
    public class ApiServiceSetupCreateResult : BaseApiResponse
    {
        /// <summary>
        /// Gets or sets the service setup identifier.
        /// </summary>
        /// <value>
        /// The service setup identifier.
        /// </value>
        public int ServiceSetupID { get; set; }
    }

    /// <summary>
    /// This class will use to get response from insert/update operation of priority of Asset Requirement
    /// </summary>
    public class ReorderAssetRequirementResult
    {
        /// <summary>
        /// Gets or sets the priority.
        /// </summary>
        /// <value>
        /// The priority.
        /// </value>
        public long Priority { get; set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public string Message { get; set; }
    }

    /// <summary>
    /// This class will use to get response from insert/update operation of Field Validation
    /// </summary>
    public class CreateUpdateFieldValidationDetailResult : BaseApiResponse
    {
        /// <summary>
        /// Gets or sets the validation identifier.
        /// </summary>
        /// <value>
        /// The validation identifier.
        /// </value>
        public long ValidationID { get; set; }
    }

    /// <summary>
    /// This class will use to get response from insert/update operation of FAQ category
    /// </summary>
    public class ApiCreateFaqCategory : BaseApiResponse
    {
        /// <summary>
        /// Gets or sets the FAQ category identifier.
        /// </summary>
        /// <value>
        /// The FAQ category identifier.
        /// </value>
        public int FAQCategoryId { get; set; }
    }

    /// <summary>
    /// This class will use to get response from insert/update operation of FAQ questions
    /// </summary>
    public class ApiCreateFaqQuestion : BaseApiResponse
    {
        /// <summary>
        /// Gets or sets the FAQ question identifier.
        /// </summary>
        /// <value>
        /// The FAQ question identifier.
        /// </value>
        public int FaqQuestionId { get; set; }
    }

    /// <summary>
    /// This class will use to get response from insert/update operation of Order cost
    /// </summary>
    public class ApiOrderCost : BaseApiResponse
    {
        /// <summary>
        /// Gets or sets the total cost.
        /// </summary>
        /// <value>
        /// The total cost.
        /// </value>
        public decimal? TotalCost { get; set; }
    }
    #endregion

    /// <summary>
    /// This class will use to get response from select operation of provider
    /// </summary>
    public class ApiProvider
    {
        /// <summary>
        /// Gets or sets the provider identifier.
        /// </summary>
        /// <value>
        /// The provider identifier.
        /// </value>
        public long ProviderID { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }
    }

    /// <summary>
    /// This class will use to get response from select operation of Platform
    /// </summary>
    public class ApiGetPlatforms
    {
        /// <summary>
        /// Gets or sets the name of the service.
        /// </summary>
        /// <value>
        /// The name of the service.
        /// </value>
        public string ServiceName { get; set; }

        /// <summary>
        /// Gets or sets the service identifier.
        /// </summary>
        /// <value>
        /// The service identifier.
        /// </value>
        public long ServiceID { get; set; }

        /// <summary>
        /// Gets or sets the small_ image.
        /// </summary>
        /// <value>
        /// The small_ image.
        /// </value>
        public string Small_Image { get; set; }

        /// <summary>
        /// Gets or sets the large_ image.
        /// </summary>
        /// <value>
        /// The large_ image.
        /// </value>
        public string Large_Image { get; set; }

        /// <summary>
        /// Gets or sets the small_ image_ deactive.
        /// </summary>
        /// <value>
        /// The small_ image_ deactive.
        /// </value>
        public string Small_Image_Deactive { get; set; }

        /// <summary>
        /// Gets or sets the large_ image_ deactive.
        /// </summary>
        /// <value>
        /// The large_ image_ deactive.
        /// </value>
        public string Large_Image_Deactive { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ApiGetPlatforms" /> is selected.
        /// </summary>
        /// <value>
        ///   <c>true</c> if selected; otherwise, <c>false</c>.
        /// </value>
        public bool Selected { get; set; }

        /// <summary>
        /// Gets or sets the portal status identifier.
        /// </summary>
        /// <value>
        /// The portal status identifier.
        /// </value>
        public int? PortalStatusID { get; set; }

        /// <summary>
        /// Gets or sets the previous selected order.
        /// </summary>
        /// <value>
        /// The previous selected order.
        /// </value>
        public string PreviousSelectedOrder { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is pitch platform.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is pitch platform; otherwise, <c>false</c>.
        /// </value>
        public bool IsPitchPlatform { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is pending.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is pending; otherwise, <c>false</c>.
        /// </value>
        public bool IsPending { get; set; }

        /// <summary>
        /// Gets or sets the notes.
        /// </summary>
        /// <value>
        /// The notes.
        /// </value>
        public string Notes { get; set; }

        /// <summary>
        /// Gets or sets the pitch cost.
        /// </summary>
        /// <value>
        /// The pitch cost.
        /// </value>
        public string PitchCost { get; set; }

        /// <summary>
        /// Gets or sets the required aproval text.
        /// </summary>
        /// <value>
        /// The required aproval text.
        /// </value>
        public string RequiredApprovalText { get; set; }

        /// <summary>
        /// Gets or sets a value indicating QuiverSalesAgent.
        /// </summary>
        /// <value>
        /// <c>true</c> if this QuiverSalesAgent; otherwise, <c>false</c>.
        /// </value>
        public bool? QuiverSalesAgent { get; set; }

        /// <summary>
        /// Gets or sets the is edit mode.
        /// </summary>
        /// <value>
        /// The is edit mode.
        /// </value>
        public bool? IsEditMode { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is client visible.
        /// </summary>
        /// <value><c>true</c> if this instance is client visible; otherwise, <c>false</c>.</value>
        public bool IsDisplay { get; set; }
    }

    /// <summary>
    /// This class will use to get response from select operation of Order
    /// </summary>
    public class ApiGetOrder : BaseApiResponse
    {
        /// <summary>
        /// Gets or sets the title identifier.
        /// </summary>
        /// <value>
        /// The title identifier.
        /// </value>
        public long TitleID { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the type of the film.
        /// </summary>
        /// <value>
        /// The type of the film.
        /// </value>
        public string FilmType { get; set; }

        /// <summary>
        /// Gets or sets the type of the title.
        /// </summary>
        /// <value>
        /// The type of the title.
        /// </value>
        public string TitleType { get; set; }

        /// <summary>
        /// Gets or sets the runtime.
        /// </summary>
        /// <value>
        /// The runtime.
        /// </value>
        public int Runtime { get; set; }

        /// <summary>
        /// Gets or sets the year.
        /// </summary>
        /// <value>
        /// The year.
        /// </value>
        public int Year { get; set; }

        /// <summary>
        /// Gets or sets the language code.
        /// </summary>
        /// <value>
        /// The language code.
        /// </value>
        public string LanguageCode { get; set; }

        /// <summary>
        /// Gets or sets the language.
        /// </summary>
        /// <value>
        /// The language.
        /// </value>
        public string Language { get; set; }

        /// <summary>
        /// Gets or sets the territories by service.
        /// </summary>
        /// <value>
        /// The territories by service.
        /// </value>
        public List<ApiGetOrderServicesTerritory> TerritoriesByService { get; set; }

        /// <summary>
        /// Gets or sets the metadata language.
        /// </summary>
        /// <value>
        /// The metadata language.
        /// </value>
        public string MetadataLanguage { get; set; }

        /// <summary>
        /// Gets or sets the provider identifier.
        /// </summary>
        /// <value>
        /// The provider identifier.
        /// </value>
        public long? ProviderID { get; set; }

        /// <summary>
        /// Gets or sets the site identifier.
        /// </summary>
        /// <value>
        /// The site identifier.
        /// </value>
        public int? SiteID { get; set; }

        /// <summary>
        /// Gets or sets the provider.
        /// </summary>
        /// <value>
        /// The provider.
        /// </value>
        public string Provider { get; set; }

        /// <summary>
        /// Gets or sets the order status identifier.
        /// </summary>
        /// <value>
        /// The order status identifier.
        /// </value>
        public int? OrderStatusID { get; set; }

        /// <summary>
        /// Gets or sets the created date.
        /// </summary>
        /// <value>
        /// The created date.
        /// </value>
        public DateTime? SubmittedDate { get; set; }

        /// <summary>
        /// Gets or sets the selected territory count.
        /// </summary>
        /// <value>
        /// The selected territory count.
        /// </value>
        public int? SelectedTerritoryCount { get; set; }

        /// <summary>
        /// Gets or sets the selected language.
        /// </summary>
        /// <value>
        /// The selected language.
        /// </value>
        public List<string> SelectedLanguage { get; set; }

    }



    /// <summary>
    /// This class will use to get response from select operation of Order service territory
    /// </summary>
    public class ApiGetOrderServicesTerritory
    {
        /// <summary>
        /// Gets or sets the services.
        /// </summary>
        /// <value>
        /// The services.
        /// </value>
        public long Services { get; set; }

        /// <summary>
        /// Gets or sets the territory.
        /// </summary>
        /// <value>
        /// The territory.
        /// </value>
        public List<GetOrderServicesTerritory> Territory { get; set; }
    }

    /// <summary>
    /// This class will use to get response from select operation of Order service territory
    /// </summary>
    public class GetOrderServicesTerritory
    {
        /// <summary>
        /// Gets or sets the territory identifier.
        /// </summary>
        /// <value>
        /// The territory identifier.
        /// </value>
        public long TerritoryID { get; set; }
    }

    /// <summary>
    /// This class will use to get response from insert/update operation of Price Tier
    /// </summary>
    public class APriceTierCreateResultWithId : BaseApiResponse
    {
        /// <summary>
        /// Gets or sets the ptid.
        /// </summary>
        /// <value>
        /// The ptid.
        /// </value>
        public long PTID { get; set; }
    }

    /// <summary>
    /// Class AssetParameterAddEdit contains all AssetParameterAddEdit related methods and variable.
    /// </summary>
    public class AssetParameterAddEdit : BaseApiResponse
    {
        /// <summary>
        /// Gets or sets the asset parameter identifier.
        /// </summary>
        /// <value>The asset parameter identifier.</value>
        public long AssetParameterId { get; set; }
    }

    /// <summary>
    /// This class will use to get response from insert/update operation of Currency Tier
    /// </summary>
    public class ApiCurrencyTierCreateResultWithId : BaseApiResponse
    {
        /// <summary>
        /// Gets or sets the ctid.
        /// </summary>
        /// <value>
        /// The ctid.
        /// </value>
        public long CTID { get; set; }
    }

    /// <summary>
    /// This class will use to get response from insert/update operation of Order Submitted details
    /// </summary>
    public class ResponseOrderSubmitted : BaseApiResponse
    {
        /// <summary>
        /// Gets or sets the display title.
        /// </summary>
        /// <value>
        /// The display title.
        /// </value>
        public string DisplayTitle { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is subscription applied.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is subscription applied; otherwise, <c>false</c>.
        /// </value>
        public bool IsSubscriptionApplied { get; set; }
    }

    /// <summary>
    /// Class FieldReadOnlyResponse.
    /// </summary>
    public class FieldReadOnlyResponse : BaseApiResponse
    {
        /// <summary>
        /// Gets or sets a value indicating whether this instance is field read only.
        /// </summary>
        /// <value><c>true</c> if this instance is field read only; otherwise, <c>false</c>.</value>
        public bool ISFieldReadOnly { get; set; }
    }

    /// <summary>
    /// Class GetTimeSaverFlagResponse.
    /// </summary>
    public class GetTimeSaverFlagResponse : BaseApiResponse
    {
        /// <summary>
        /// Gets or sets a value indicating whether [sales date time saver flag].
        /// </summary>
        /// <value><c>true</c> if [sales date time saver flag]; otherwise, <c>false</c>.</value>
        public bool SalesDateTimeSaverFlag { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [pricing time saver flag].
        /// </summary>
        /// <value><c>true</c> if [pricing time saver flag]; otherwise, <c>false</c>.</value>
        public bool PricingTimeSaverFlag { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [rating time saver flag].
        /// </summary>
        /// <value><c>true</c> if [rating time saver flag]; otherwise, <c>false</c>.</value>
        public bool RatingTimeSaverFlag { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [close popups flag].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [close popups flag]; otherwise, <c>false</c>.
        /// </value>
        public bool ClosePopupsFlag { get; set; }
    }

    /// <summary>
    /// Asset Or Carrier
    /// </summary>
    public class AssetOrCarrier : BaseApiResponse
    {
        /// <summary>
        /// Gets or sets the asset or carrier.
        /// </summary>
        /// <value>
        /// The asset or carrier.
        /// </value>
        public int IsAssetOrCarrier { get; set; }
    }

    /// <summary>
    /// Class IsQuestionariesService contains all Is questionaries service related properties. 
    /// </summary>
    public class QuestionariesService : BaseApiResponse
    {
        /// <summary>
        /// Gets or sets a value indicating whether this instance is QUESTIONARIES service.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is QUESTIONARIES service; otherwise, <c>false</c>.
        /// </value>
        public bool IsQuestionariesService { get; set; }
    }

    /// <summary>
    /// Class ServiceIDArray contains all Service identifier array related properties.
    /// </summary>
    public class ServiceIDArray
    {
        /// <summary>
        /// Gets or sets the service identifier.
        /// </summary>
        /// <value>
        /// The service identifier.
        /// </value>
        public int[] serviceID { get; set; }
    }

    /// <summary>
    /// Class MetadataAndAssetRemainingCount contains all Metadata and asset remaining count related properties. 
    /// </summary>
    public class MetadataAndAssetRemainingCount : BaseApiResponse
    {
        /// <summary>
        /// Gets or sets the metadata remaining count.
        /// </summary>
        /// <value>
        /// The metadata remaining count.
        /// </value>
        public int MetadataRemainingCount { get; set; }

        /// <summary>
        /// Gets or sets the asset remaining count.
        /// </summary>
        /// <value>
        /// The asset remaining count.
        /// </value>
        public int AssetRemainingCount { get; set; }
    }

    /// <summary>
    /// This class will use to get response from insert/update operation of Currency Tier
    /// </summary>
    public class ApiDiscountCouponCreateResultWithId : BaseApiResponse
    {
        /// <summary>
        /// Gets or sets the ctid.
        /// </summary>
        /// <value>
        /// The ctid.
        /// </value>
        public long CouponId { get; set; }
    }

    public class ApiDiscountCouponCode : BaseApiResponse
    {
        /// <summary>
        /// Gets Or Sets the coupon Code
        /// </summary>
        public string DiscountCode { get; set; }

        /// <summary>
        /// Gets Or Sets the ResultSuccess
        /// </summary>
        public bool? ResultSuccess { get; set; }

        /// <summary>
        /// Gets Or Sets the ResultMessage
        /// </summary>
        public string ResultMessage { get; set; }
    }

    public class Response
    {
        /// <summary>
        /// Gets or sets a value indicating whether response is success or fail
        /// </summary>
        /// <value>
        ///   <c>true</c> if success; otherwise, <c>false</c>.
        /// </value>
        public bool Success { get; set; }

        /// <summary>
        /// Gets or sets Message
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the taid.
        /// </summary>
        /// <value>
        /// The taid.
        /// </value>
        public long TAID { get; set; }
    }

    public class GetByteArray : BaseApiResponse
    {
        /// <summary>
        /// Gets or sets the byte information.
        /// </summary>
        /// <value>
        /// The byte information.
        /// </value>
        public byte[] ByteInfo { get; set; }
    }
}
