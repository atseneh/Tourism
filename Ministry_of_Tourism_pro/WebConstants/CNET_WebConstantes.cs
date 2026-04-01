namespace Ministry_of_Tourism_pro.WebConstants
{
    public static class CNET_WebConstantes
    {
        public static string ClaimsIssuer => "cnetERP";
        public static string CookieScheme => "cnet.erp.v6";
        public static string IdentificationCookie => "cnet.erp.v6.myId";
        public const int IdentificationCookieLifeTime = 10080;
        public const int IdentificationCookieDailyLifeTime = 1440;
        public const int USER_COMPONENET = 745;
        public const int CHANGE_PASSWORD = 1663;
        public const int ACTIVITY_DEFINATION_PREPARED = 1001;
        public const string HARDCODED_TIN = "0000000000-00";
        public const int HARDCODED_BRANCH = 1;
        public const int EMPLOYEE_CATEGORY = 2;
        public const int ORGANIZATION_GSL_TYPE = 28;
        public const int PERSON_GSL_TYPE = 26;
        
        // Hotel Registry Categories (Identification Types)
        public const int CAT_PROPERTY_PROFILE = 3229;
        public const int CAT_ROOM_INVENTORY = 3230;
        public const int CAT_FOOD_BEVERAGE_RETAIL = 3231;
        public const int CAT_MEETINGS_EVENTS = 3232;
        public const int CAT_PUBLIC_FACILITIES = 3233;
        public const int CAT_ACCESSIBILITY = 3234;
        public const int CAT_SECURITY_SAFETY = 3235;
        public const int CAT_TRANSPORT_PARKING = 3236;
        public const int CAT_ICT_GUEST_SERVICES = 3237;
        public const int CAT_UTILITIES_RESILIENCE = 3238;
        public const int CAT_SUSTAINABILITY_CERTIFICATIONS = 3239;
        public const int CAT_STAFFING_LANGUAGES = 3240;

        //roles
        public const int HEAD_OFFICE = 1;
        public const int SYSTEM_ADMINISTRATOR = 2;
        public const int MAIN_CONSIGNEE = 3;
        public const int ADMINISTRATOR = 4;
        public const int USER_1 = 5;
        public const int USER_2 = 6;
        public const int SUPERVISOR = 7;
        public const int GENERAL_MANAGER = 8;

        /// <summary>Tourism establishment business types used in HotelOwner registry forms.</summary>
        public static readonly List<string> BusinessTypes = new()
        {
            "Hotel",
            "Motel",
            "Lodge",
            "Resort",
            "Guest House",
            "Pension",
            "Hostel",
            "Boutique Hotel",
            "Eco Lodge",
            "Safari Camp",
            "Serviced Apartment",
            "Villa / Chalet",
            "Heritage Inn",
            "Conference Hotel",
            "Airport Hotel"
        };

        /// <summary>Attachment categories for hotel registry documents.</summary>
        public static readonly List<string> AttachmentCategories = new()
        {
            "Business License",
            "Tax Certificate (TIN)",
            "Property Photos",
            "Health & Safety Certificate",
            "Fire Safety Certificate",
            "Environmental Permit",
            "Other"
        };
    }
}
