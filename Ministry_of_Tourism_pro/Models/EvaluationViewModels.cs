namespace Ministry_of_Tourism_pro.Models
{
    public class EvaluationQuestion
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string QuestionText { get; set; } = string.Empty;
        public string? SubText { get; set; }
        public string? Status { get; set; } // "yes", "no", or null
        public string? Remarks { get; set; }
    }

    public class EvaluationCategory
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Icon { get; set; } = "fa-clipboard-list";
        public List<EvaluationQuestion> Questions { get; set; } = new List<EvaluationQuestion>();
    }

    public class EvaluationViewModel
    {
        public string EstablishmentName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Grade { get; set; } = string.Empty;
        public string EvaluatorName { get; set; } = string.Empty;
        public string Date { get; set; } = string.Empty;
        public List<EvaluationCategory> Categories { get; set; } = new List<EvaluationCategory>();
    }

    public static class EvaluationMockData
    {
        public static List<EvaluationCategory> GetCategories()
        {
            return new List<EvaluationCategory>
    {
        //new EvaluationCategory {
        //    Id = 1, Name = "አጠቃላይ መረጃና ሁኔታዎች", Icon = "fa-info-circle",
        //    Questions = new List<EvaluationQuestion> {
        //        new EvaluationQuestion { Id = 101, Code = "1", QuestionText = "የተቋሙ ስም እና አድራሻ ተመዝግቧል?" },
        //        new EvaluationQuestion { Id = 102, Code = "2", QuestionText = "የባለፈቃዱ ስም እና አድራሻ ተመዝግቧል?" },
        //        new EvaluationQuestion { Id = 103, Code = "3", QuestionText = "የመኝታ ክፍል ብዛት ተመዝግቧል?" },
        //        new EvaluationQuestion { Id = 104, Code = "4", QuestionText = "የአልጋ ክፍል ብዛት ተመዝግቧል?" },
        //        new EvaluationQuestion { Id = 105, Code = "5", QuestionText = "የአዳራሽ ብዛት ተመዝግቧል?" }
        //    }
        //},
        new EvaluationCategory {
            Id = 2, Name = "የሆቴሉ ውስጣዊና ውጫዊ ገፅታ", Icon = "fa-building",
            Questions = new List<EvaluationQuestion> {
                new EvaluationQuestion { Id = 201, Code = "3.1", QuestionText = "ሆቴሉ እንግዶች በቀላሉ እንዲያገኙት መግቢያ መንገድ፣ የአቅጣጫ ምልክት እንዲሁም በግልፅ የሚታይ ሀገርኛ ስያሜ አለው? (ቼን ያላቸውን አይመለከትም)" },
                new EvaluationQuestion { Id = 202, Code = "3.2", QuestionText = "የሆቴሉ ዋና መግቢያና መውጫ ለእንግዶች ብቻ እና ለሠራተኞች የተለየ ነው?" },
                new EvaluationQuestion { Id = 203, Code = "3.3", QuestionText = "የውስጥ ይዘታው አደረጃጀቱና በውስጡ የሚገኙ የማስጌጫና መገልገያ ዕቃዎች ጥራታቸው ጥሩ የሆነ ውብና ማራኪ ነው?" },
                new EvaluationQuestion { Id = 204, Code = "3.4", QuestionText = "የግድግዳው የመስታወቶቹ፣ የቢሮች መስኮቶች ይዞታና የጥራት ደረጃ ከፍተኛ ነው?" },
                new EvaluationQuestion { Id = 205, Code = "3.5", QuestionText = "ህንፃው ከ4 ፎቅ በላይ ከሆነ ቢያንስ አንድ አሳንሰር አለ?" },
                new EvaluationQuestion { Id = 206, Code = "3.6", QuestionText = "ኮርደሩ ድምፅ የማያስተጋባና ስፋቱ ከ1.50 ሜትር ያላነሰ ነው?" }
            }
        },
        new EvaluationCategory {
            Id = 3, Name = "የእንግዳ ማረፊያ (Lobby Area)", Icon = "fa-couch",
            Questions = new List<EvaluationQuestion> {
                new EvaluationQuestion { Id = 301, Code = "4.1", QuestionText = "በእንግዳ መቀበያ ክፍል አካባቢ ከፍተኛ ጥራት ምቾት ያላቸው መቀመጫዎች ወይም ሶፋዎች ጠረጴዛዎችና ቴሌቭዥን አሉ?" },
                new EvaluationQuestion { Id = 302, Code = "4.2", QuestionText = "የኢንተርኔትን የፅሕፈት አገልግሎት የሚሰጥ ቢዝነስ ሴንተር አለ?" },
                new EvaluationQuestion { Id = 303, Code = "4.3", QuestionText = "እንደ አየሩ ሁኔታ መቆጣጠር የሚያስችል ኤርኮንድሽነር አለ?" },
                new EvaluationQuestion { Id = 304, Code = "4.4", QuestionText = "የሻንጣ ማመላለስና ማቆየት፣ የመረጃና ተጓዳኝ አገልግሎት የሚሰጥ የተለየ ክፍል (Concierge Desk) አለ?" },
                new EvaluationQuestion { Id = 305, Code = "4.5", QuestionText = "ለሴትና ለወንድ የተለየ ለእንግዶች በቂ የሆነ የመፀዳጃ ክፍል አለ?" }
            }
        },
        new EvaluationCategory {
            Id = 4, Name = "የእንግዳ መቀበያ ክፍል (Front Office/Reception)", Icon = "fa-desk",
            Questions = new List<EvaluationQuestion> {
                new EvaluationQuestion { Id = 401, Code = "5.1", QuestionText = "በኮምፒውተር የተደገፈ የሪዘርቬሽን፣ የእንግዶች መመዝገቢያ፣ የሂሳብና የመረጃ አያያዝ ዘዴ አለ?" },
                new EvaluationQuestion { Id = 402, Code = "5.2", QuestionText = "የእንግዶች ንብረትና ገንዘብ ማስቀመጫ ካዝናና ደረሰኝ አለ?" },
                new EvaluationQuestion { Id = 403, Code = "5.3", QuestionText = "ከኢትዮጵያ ብር በተጨማሪ በኢትዮጵያ ብሔራዊ ደንብ መሰረት በታወቁ የውጭ አገር ገንዘቦች ምንዛሬና የክሬዲት ካርድ አገልግሎት አለ?" },
                new EvaluationQuestion { Id = 404, Code = "5.4", QuestionText = "የእንግዳ መቀበያ ክፍል ሃላፊ በሆቴል ሙያ ቢያንስ በዲግሪ የተመረቀ እና ሌሎች ሰራተኞች ቢያንስ ዲፕሎማ ያላቸው ናቸው?" },
                new EvaluationQuestion { Id = 405, Code = "5.5", QuestionText = "24 ሰዓት ለእንግዶች የቴሌፎን አገልግሎት አለ?" },
                new EvaluationQuestion { Id = 406, Code = "5.6", QuestionText = "ቢያንስ አራት አገራት ሰዓት አቆጣጠር የሚያሳይ የግድግዳ ሰዓት በሚታይ ቦታ አለ?" },
                new EvaluationQuestion { Id = 407, Code = "5.7", QuestionText = "የተሟላ መጀመሪያ ሕክምና እርዳታ መሽጫ ሳጥን አለ?" }
            }
        },
        new EvaluationCategory {
            Id = 5, Name = "የእንግዳ መኝታ ክፍሎች (Guest Bed Rooms)", Icon = "fa-bed",
            Questions = new List<EvaluationQuestion> {
                new EvaluationQuestion { Id = 501, Code = "6.1.1", QuestionText = "ጠቅላላ የእንግዶች ክፍሎች 5% ስዊት ክፍል ያለው የመኝታ ክፍሉ ስፋቱ 16 ካ.ሜ ያላነሰ፤ የመታጠቢያ ክፍል ስፋቱ 9 ካ.ሜ ያላነሰ እንዲሁም የጃኩዝ አገልግሎት አለ?" },
                new EvaluationQuestion { Id = 502, Code = "6.1.2", QuestionText = "የክፍሉ ሃላፊ በሆቴል አስተዳደር ወይም በቤት አያያዝ (House Keeping) የመጀመሪያ ዲግሪ ያለው እንዲሁም ሌሎች ሰራተኞች በቤት አያያዝ የሰለጠኑ ናቸው?" },
                new EvaluationQuestion { Id = 503, Code = "6.1.3", QuestionText = "ቢያንስ 10 የመኝታ ክፍሎች አሉ?" },
                new EvaluationQuestion { Id = 504, Code = "6.1.4", QuestionText = "የመኝታ ክፍሉ ስፋት መፀዳጃው ቤቱንና መግቢያውን ሳይጨምር ለነጠላ አልጋ ክፍል 12 ካ.ሜ ለጥንድ ወይም ድብል ክፍል 16 ካ.ሜ ነው?" },
                new EvaluationQuestion { Id = 505, Code = "6.1.5", QuestionText = "የአልጋ ስፋት ቢያንስ ለነጠላ 90 ሴ.ሜ በ200 ሴ.ሜ ለደብል 120 በ200 ሳ.ሜ የሆነ ነው?" },
                new EvaluationQuestion { Id = 506, Code = "6.1.6", QuestionText = "አልጋው ምቾት ያለው፣ የስፕሪንግ ፍራሽ ከተስማሚ ትራስ ጋር አለ?" },
                new EvaluationQuestion { Id = 507, Code = "6.1.7", QuestionText = "ቢያንስ ለአንድ አልጋ ሶስት የአንሶላ ቅያሪ ያለው፣ በየቀኑ ወይም እንግዳው በጠየቀው ጊዜ ሊቀየር የሚችል እንዲሁም በቂ ቅያሬና ምቾት ያላቸው ብርድ ልብሶችና የአልጋ ልብሶች አሉ?" },
                new EvaluationQuestion { Id = 508, Code = "6.1.8", QuestionText = "በክፍሎቹ ውስጥ የተሸፈኑ በቂ መብራቶች አሉ?" },
                new EvaluationQuestion { Id = 509, Code = "6.1.9", QuestionText = "ለእያንዳንዱ አልጋ ሁለት የራስጌ መብራትና ከሞዲኖ አለ?" },
                new EvaluationQuestion { Id = 510, Code = "6.1.10", QuestionText = "በመግቢያና ከአልጋ አጠገብ መብራት ማብሪያ ማጥፊያ አለ?" },
                new EvaluationQuestion { Id = 511, Code = "6.1.11", QuestionText = "የኤሌክትሪክ ቮልቴጅ የሚያሳዩ ሶኬቶች አሉ?" },
                new EvaluationQuestion { Id = 512, Code = "6.1.12", QuestionText = "ሲፈለግ የህፃናት አልጋ (Baby Cot) እና ተጨማሪ አልጋ (extra bed) አለ?" },
                new EvaluationQuestion { Id = 513, Code = "6.1.13", QuestionText = "የመኝታ ክፍሎቹ ተስማሚ የሆኑና ውበት የሚሰጡ መጋረጃና ምንጣፍ አላቸው?" },
                new EvaluationQuestion { Id = 514, Code = "6.1.14", QuestionText = "የክፍሎቹ በር ከውስጥ ወደ ውጭ ማመልከቻ ቀዳዳ የደህንነት ሰንሰለት ወይም ለክፍሉ ተስማሚ የሆነ መጋረጃ አለ?" },
                new EvaluationQuestion { Id = 515, Code = "6.1.15", QuestionText = "በሩ በኤሌክትሮኒክ ካርድ ወይም ሁለት ጊዜ ሊቆለፍ የሚችል ነው?" },
                new EvaluationQuestion { Id = 516, Code = "6.1.16", QuestionText = "ለእያንዳንዱ የመኝታ ክፍል የሚከፈት መስኮት አለ?" },
                new EvaluationQuestion { Id = 517, Code = "6.1.17", QuestionText = "በክፍሉ በር በውጭ በኩል የክፍሉ ቁጥር በግልፅ ይታያል?" },
                new EvaluationQuestion { Id = 518, Code = "6.1.18", QuestionText = "በክፍሉ ውስጥ ክፍሉ የሚገኝበት ቦታ፣ የእሳት አደጋ ማምለጫዎችና ሊከተሉት የሚገባ ቅድምተከተል፣ የሆቴሉ ሕግና ደንብ፣ የምግብና መጠጥ አገልግሎት ዝርዝርና ዋጋ፣ ሜኑ፣ የላውንደሪ አገልግሎት ወዘተ በአማርኛና በእንግሊዘኛ የተፃፈ አለ?" },
                new EvaluationQuestion { Id = 519, Code = "6.1.19", QuestionText = "እንደክፍሎቹ ዓይነትና አልጋ ብዛት የጥራት ደረጃቸውና ምቾታቸው ከፍተኛ የሆነ በቂ ወንበሮችና ጠረጴዛዎች አሉ?" },
                new EvaluationQuestion { Id = 520, Code = "6.1.20", QuestionText = "ከክፍሉ ጋር የተሰራ ቢያንስ 8 መስቀያዎች ያሉት እንዲሁም መሳቢያዎችና መደርደሪያዎች ያሉት ቁምሳጥን (built-in cupboard) አለ?" },
                new EvaluationQuestion { Id = 521, Code = "6.1.21", QuestionText = "እንግዶች ክፍል ውስጥ ማረፍ ወይም መስራት ሲፈልጉ ሊጠቀሙባቸው የሚችሉ ምልክቶች (do not disturb / Make up the Room) አሉ?" },
                new EvaluationQuestion { Id = 522, Code = "6.1.22", QuestionText = "በመኝታ ክፍሉ ውስጥ የስልክ አገልግሎትና የመቀስቀሻ ጥሪ አገልግሎት (Wake-up Call) አለ?" },
                new EvaluationQuestion { Id = 523, Code = "6.1.23", QuestionText = "የተለያዩ ቻናል ያለው ባለከለር ቴሌቭዥን በየክፍሉ አለ?" },
                new EvaluationQuestion { Id = 524, Code = "6.1.24", QuestionText = "በክፍሉ ውስጥ የኢንተርኔት አገልግሎት አለ?" },
                new EvaluationQuestion { Id = 525, Code = "6.1.25", QuestionText = "ቆሻሻ ማጠራቀሚያ ባለክዳን ቅርጫት አለ?" }
            }
        },
        new EvaluationCategory {
            Id = 6, Name = "የመታጠቢያ ክፍሎች (Bathrooms)", Icon = "fa-bath",
            Questions = new List<EvaluationQuestion> {
                new EvaluationQuestion { Id = 601, Code = "6.2.1", QuestionText = "ለሁሉም የመኝታ ክፍሎች የራሳቸው መታጠቢያና መፀዳጃ ክፍሎች አሉ?" },
                new EvaluationQuestion { Id = 602, Code = "6.2.2", QuestionText = "መፀዳጃ ክፍሉ ስፋቱ ከ6 ካ.ሜ በላይ ነው?" },
                new EvaluationQuestion { Id = 603, Code = "6.2.3", QuestionText = "የእጅና የፊት መታጠቢያና የመፀዳጃ መቀመጫ አለ?" },
                new EvaluationQuestion { Id = 604, Code = "6.2.4", QuestionText = "በማንኛውም ጊዜ የቀዝቃዛና የሙቅ ውሃ አገልግሎት የሚሰጥና ባለሚክሰር ሻወርና መታጠቢያ ገንዳ አለ?" },
                new EvaluationQuestion { Id = 605, Code = "6.2.5", QuestionText = "ሻወሩ መጋረጃ ወይም የሚዘረጋ የሚከፈት በር አለው?" },
                new EvaluationQuestion { Id = 606, Code = "6.2.6", QuestionText = "ወለሉና ግድግዳው በሴራሚክ ተሸፍኗል?" },
                new EvaluationQuestion { Id = 607, Code = "6.2.7", QuestionText = "መኝታና መታጠቢያ ክፍሉን የሚለይ የሚቆለፍ በር አለ?" },
                new EvaluationQuestion { Id = 608, Code = "6.2.8", QuestionText = "ሽታን ስቦ ማስወጣት የሚችል መሳሪያ አለ?" },
                new EvaluationQuestion { Id = 609, Code = "6.2.9", QuestionText = "ከወገብ በላይ የሚያሳይ መስታወት አለ?" },
                new EvaluationQuestion { Id = 610, Code = "6.2.10", QuestionText = "ክፍሉ እንደሚይዘው እንግዳ ብዛት ደረጃቸውን የጠበቁ የፎጣዎች መደርደሪያ ወይም ማስቀመጫ አለው?" },
                new EvaluationQuestion { Id = 611, Code = "6.2.11", QuestionText = "ልብስ መስቀያ ወይም ማንጠልጠያ አለ?" },
                new EvaluationQuestion { Id = 612, Code = "6.2.12", QuestionText = "የጥራት ደረጃቸው ከፍተኛ የሆነ የንፅህና መገልገያዎች (በቂ ሳሙና፣ የመፀዳጃ ወረቀት፣ የሻወር ቆብ፣ ሻምፖ፣ የጥርስ ብሩሽ እንዲሁም በጥያቄ የሚቀርብ የፀጉር ማድረቂያ ማሽን ወዘተ) አሉ?" },
                new EvaluationQuestion { Id = 613, Code = "6.2.13", QuestionText = "ክዳን ያለው የቆሻሻ ማጠራቀሚያ አለ?" },
                new EvaluationQuestion { Id = 614, Code = "6.2.14", QuestionText = "በቂ መብራትና ሶኬቶች አሉ?" }
            }
        },
        new EvaluationCategory {
            Id = 7, Name = "ምግብ ቤት (Restaurant)", Icon = "fa-utensils",
            Questions = new List<EvaluationQuestion> {
                new EvaluationQuestion { Id = 701, Code = "7.1", QuestionText = "የቁርስ፣ የምሳና የእራት አገልግሎት የሚሰጥ በአንድ ጊዜ ቢያንስ የሆቴሉን የመኝታ ክፍል እንግዶች ሊያስተናግድ የሚችል ምግብ ቤት አለ?" },
                new EvaluationQuestion { Id = 702, Code = "7.2", QuestionText = "ከምግብ ቤቱ አቅራቢያ ጥሩ የጥራት ደረጃ ያለው የመታጠቢያና የመፀዳጃ ክፍል አለ?" },
                new EvaluationQuestion { Id = 703, Code = "7.3", QuestionText = "ሰፊ ምርጫ ያላቸው የተለያዩ የአገር ውስጥና ዓለም አቀፍ ምግቦችና መጠጦች ያቀርባል?" },
                new EvaluationQuestion { Id = 704, Code = "7.4", QuestionText = "የተለያዩ የምግብና መጠጦች ዝርዝር ከነዋጋቸው የሚያሳይ ደረጃውን የጠበቀ አላካርትና ዕለታዊ ሜኑ አለ?" },
                new EvaluationQuestion { Id = 705, Code = "7.5", QuestionText = "የጥራት ደረጃቸውን የጠበቁና ምቾትና ጥንካሬ ያላቸው ወንበሮችና ጠረጴዛዎች አሉ?" },
                new EvaluationQuestion { Id = 706, Code = "7.6", QuestionText = "ለአገልግሎት አሰጣጥ ቅልጥፍና የሚረዳ የሰርቪስ ቦታ (Service Station) አለ?" },
                new EvaluationQuestion { Id = 707, Code = "7.7", QuestionText = "የማይዝጉ የጥራት ደረጃቸው ጥሩ የሆነ የመመገቢያ፣ የመጠጥ የማቅረቢያ መገልገያዎች አሉ?" },
                new EvaluationQuestion { Id = 708, Code = "7.8", QuestionText = "የምግብ ቤቶቹ ወለል፣ ግድግዳ፣ ጣሪያ፣ በሮች፣ መስኮቶችና የውስጥ ዕቃዎች ለእንግዶች ደህንነት አገልግሎትና ምቾት ጥሩ ደረጃ ላይ ናቸው?" },
                new EvaluationQuestion { Id = 709, Code = "7.9", QuestionText = "ልዩ ፍላጎት ላላቸው እንግዶች የተመቸ ነው?" },
                new EvaluationQuestion { Id = 710, Code = "7.10", QuestionText = "ጥሩ የጥራት ደረጃ ያላቸው የገበታ ጨርቆችና ሌሎች መገልገያ ዕቃዎች አሉ?" },
                new EvaluationQuestion { Id = 711, Code = "7.11", QuestionText = "አስፈላጊ በሆነበት ጊዜ እንደአየሩ ሁኔታ ማስተካከል የሚችል ኤርኮንዲሽነር ወይም ቬንትሌተር አለ?" }
            }
        },
        new EvaluationCategory {
            Id = 8, Name = "ቡና ቤት (Bar)", Icon = "fa-mug-saucer",
            Questions = new List<EvaluationQuestion> {
                new EvaluationQuestion { Id = 801, Code = "8.1", QuestionText = "ዘመናዊና በተሟላ ሁኔታ የተደራጀ የጥራት ደረጃቸው ጥሩ የሆነ ቢያንስ አንድ ቡና ቤት አለ?" },
                new EvaluationQuestion { Id = 802, Code = "8.2", QuestionText = "በቂና የጥራት ደረጃቸው ጥሩ የሆነ የምግብና መጠጥ መገልገያ ዕቃዎች፣ ለሚፈለገው አገልግሎት የተለየ፣ ተስማሚ የሆነና በአግባቡ የተያዙ የመስተንግዶ ቁሳቁሶች አሉ?" },
                new EvaluationQuestion { Id = 803, Code = "8.3", QuestionText = "የጥራት ደረጃው ከፍተኛ ከሆነ ማትሪያል የተሰሩ የመጠጦች መደርደሪያ አለ?" },
                new EvaluationQuestion { Id = 804, Code = "8.4", QuestionText = "የጥራት ደረጃቸው ከፍተኛ የሆኑ ማቀዝቀዣዎች፣ የቡና ማሽን፣ የባር ካውንተር፣ የእቃ ማጠቢያ ገንዳ ወዘተ አሉ?" }
            }
        },
        new EvaluationCategory {
            Id = 9, Name = "ሁለገብ አዳራሽ (Function Hall)", Icon = "fa-calendar-check",
            Questions = new List<EvaluationQuestion> {
                new EvaluationQuestion { Id = 901, Code = "9.1", QuestionText = "ቢያንስ 200 ሰዎች ማስተናገድ የሚችል አንድ ሁለገብ አዳራሽ አለ?" },
                new EvaluationQuestion { Id = 902, Code = "9.2", QuestionText = "ድምፅ የማያስተጋባና ደረጃውን የጠበቀ የሳውንድ ሲስተም የተገጠመለት ነው?" },
                new EvaluationQuestion { Id = 903, Code = "9.3", QuestionText = "በቂ ብርሃን ያለውና እንደ አየሩ ሁኔታ ሊስተካከል የሚችል ኤርኮንዲሽነር ወይም ቬንትሌተር የተገጠመለት ነው?" },
                new EvaluationQuestion { Id = 904, Code = "9.4", QuestionText = "ከአዳራሹ አጠገብ ለወንድና ለሴት የተለየ ደረጃውን የጠበቀ የእጅ መታጠቢያና መፀዳጃ አለ?" },
                new EvaluationQuestion { Id = 905, Code = "9.5", QuestionText = "በመፀዳጃ ክፍል የኮት ወይም ካፖርት መስቀያ አለ?" }
            }
        },
        new EvaluationCategory {
            Id = 10, Name = "ምግብ ዝግጅት ክፍል (Kitchen)", Icon = "fa-kitchen-set",
            Questions = new List<EvaluationQuestion> {
                new EvaluationQuestion { Id = 1001, Code = "10.1", QuestionText = "የምግብ ዝግጅት ክፍል ቢያንስ 20 ካ.ሜ ስፋት አለው?" },
                new EvaluationQuestion { Id = 1002, Code = "10.2", QuestionText = "ዋና ምግብ ዝግጅት ክፍል አደረጃጀቱ በደሴት መልክ ነው?" },
                new EvaluationQuestion { Id = 1003, Code = "10.3", QuestionText = "የምግብ ዝግጅት ክፍሉ ከምግብ ቤቱ ጎን ለጎን ሆኖ ባለመስታወት በር ነው?" },
                new EvaluationQuestion { Id = 1004, Code = "10.4", QuestionText = "ክፍሉ የጥራት ደረጃው ከፍተኛ የሆነ የጋዝና የኤሌክትሪክ ምድጃዎች፣ ለተለያዩ የምግብ ዓይነቶች የተለያዩ ማቀዝቀዣዎች፣ ማቀዝቀዣ ክፍል፣ የዕቃ ማጠቢያ ገንዳዎች፣ የተለያዩ የምግብ ማዘጋጃ ማቅረቢያና መገልገያ ዕቃዎች አሉት?" },
                new EvaluationQuestion { Id = 1005, Code = "10.5", QuestionText = "የምግብ ጥሬ ዕቃዎች ማቆያ ቦታ አለ?" },
                new EvaluationQuestion { Id = 1006, Code = "10.6", QuestionText = "የጥሬ ዕቃዎች መቀበያ፣ ለቆሻሻ ማስወገጃና ለክፍሉ ሰራተኞች መግቢያና መውጫ የውስጥ በር አለ?" },
                new EvaluationQuestion { Id = 1007, Code = "10.7", QuestionText = "ከፍተኛ በሆነ የዕቃ ማጠቢያ ማሽኖችና መሳሪያዎች የተደራጀ የዕቃ ማጠቢያ ክፍል አለ?" },
                new EvaluationQuestion { Id = 1008, Code = "10.8", QuestionText = "ክፍሉ አየርና ብርሃን ለማስገባት እንዲችል መስኮት፣ ጭስና እንፋሎት ማውጫ መሳሪያ አለ?" },
                new EvaluationQuestion { Id = 1009, Code = "10.9", QuestionText = "በአግባቡ የተቀበረ የፍሳሽ ቆሻሻ ማስወገጃና ክዳን ያለው የደረቅ ቆሻሻ ማጠራቀሚያ ቅርጫት አለ?" },
                new EvaluationQuestion { Id = 1010, Code = "10.10", QuestionText = "እንደ ሆቴሉ አገልግሎት ይዘት፣ ስፋትና ደረጃ አስተማማኝ የውሃ፣ የኤሌክትሪክና የጋዝ አቅርቦት አለ?" },
                new EvaluationQuestion { Id = 1011, Code = "10.11", QuestionText = "ግድግዳው ነጭ ሴራሚክ የለበሰ፣ ወለሉም ታጣቢ የሆነና የማያንሸራትት ዘወትር በንፅህና የተያዘ ነው?" },
                new EvaluationQuestion { Id = 1012, Code = "10.12", QuestionText = "ሙቅና ቀዝቃዛ ውሃ ያለው የእጅ መታጠቢያ አለ?" },
                new EvaluationQuestion { Id = 1013, Code = "10.13", QuestionText = "ተገቢው የፍሳሽ ማስተላለፊያ ቱቦዎች ፍሳሹ ከዋናው መስመር ጋር የሚገናኝ ወይም ተገቢው ማጠራቀሚያ የተሰራለት ነው?" },
                new EvaluationQuestion { Id = 1014, Code = "10.14", QuestionText = "ተገቢው የእሳት አደጋ መከላከያ መሳሪያዎች የተሟሉለት የመጀመሪያ ደረጃ ሕክምና መሸጫ ሳጥን አለ?" }
            }
        },
        new EvaluationCategory {
            Id = 11, Name = "ቤት አያያዝ (House Keeping)", Icon = "fa-broom",
            Questions = new List<EvaluationQuestion> {
                new EvaluationQuestion { Id = 1101, Code = "11.1", QuestionText = "ቢያንስ በየሁለት ፎቆች እራሱን የቻለ አንድ የቤት አያያዝ ክፍል በተሟላ ሁኔታ የተደራጀ አለ?" },
                new EvaluationQuestion { Id = 1102, Code = "11.2", QuestionText = "እንግዶች ለጠፋባቸው ወይም ለረሱት ንብረቶች ማስቀመጫ ቤት አለ?" },
                new EvaluationQuestion { Id = 1103, Code = "11.3", QuestionText = "የተሟላ የመጀመሪያ ሕክምና እርዳታ መስጫ ሳጥን ከነሙሉ መሳሪያ አለ?" },
                new EvaluationQuestion { Id = 1104, Code = "11.4", QuestionText = "ጊዜ ያለፈበት የእሳት አደጋ መከላከያ ቢያንስ 5 Kg Co2 አለ?" },
                new EvaluationQuestion { Id = 1105, Code = "11.5", QuestionText = "የሆቴሉ የቤት አያያዝ ተግባራትን ለማከናወን የሚያስችል የዕቃትና የዝግጅት ሂደቱን የሚገልፅ የአሰራር ስርዓት (House Keeping procedure manual) አለ?" }
            }
        },
        new EvaluationCategory {
            Id = 12, Name = "የልብስ ንጽህና ክፍል (Laundry)", Icon = "fa-soap",
            Questions = new List<EvaluationQuestion> {
                new EvaluationQuestion { Id = 1201, Code = "12.1", QuestionText = "እራሱን የቻለ ዘመናዊ የማጠቢያ፣ የማድረቂያና የመታሻ ክፍል ያለው ላውንደሪ አለ?" },
                new EvaluationQuestion { Id = 1202, Code = "12.2", QuestionText = "በቂ መደርደሪያና መስቀያዎች ያሉት የጨርቃ ጨርቆች ማደራጃ ክፍል አለ?" },
                new EvaluationQuestion { Id = 1203, Code = "12.3", QuestionText = "ከሌሎች ድርጅቶች ጋር ስምምነት በመፍጠር የደረቅ እጥበት (dry Cleaning Service) አገልግሎት ይሰጣል?" },
                new EvaluationQuestion { Id = 1204, Code = "12.4", QuestionText = "ተገቢው የፍሳሽ ማስተላለፊያ ቱቦዎች ፍሳሹ ከዋናው መስመር ጋር የሚገናኝ ወይም ተገቢው ማጠራቀሚያ የተሰራለት ነው?" }
            }
        },
        new EvaluationCategory {
            Id = 13, Name = "የዕቃ ግምጃ ቤት (Store)", Icon = "fa-boxes",
            Questions = new List<EvaluationQuestion> {
                new EvaluationQuestion { Id = 1301, Code = "13.1", QuestionText = "ለምግብና መጠጥ፣ ለጨርቃጨርቅ፣ ለጽህፈት መሳሪያዎች፣ ለጽዳት ዕቃዎች በተለያዩ ቦታዎች ለመያዝ የሚችል በአግባቡ የተያዘ በቂ ቦታ ያለው ግምጃ ቤት አለ?" },
                new EvaluationQuestion { Id = 1302, Code = "13.2", QuestionText = "የዕቃ ግምጃ ቤቱ ውስጣዊ ይዘት፣ ወለሉ፣ ግድግዳውና ጣሪያው በአግባቡ የተገነባ፣ በተገቢው ሁኔታ የተደራጀ ተገቢው የንብረት አመዘጋገብና አያያዝ ስርዓት የሚከተል ነው?" }
            }
        },
        new EvaluationCategory {
            Id = 14, Name = "የጋራ መፀዳጃ (Wash Room)", Icon = "fa-restroom",
            Questions = new List<EvaluationQuestion> {
                new EvaluationQuestion { Id = 1401, Code = "14.1", QuestionText = "ለእንግዶች በቂ የሆነ ለወንድና ለሴት የተለየ ሙቅና ቀዝቃዛ ውሃ ያለው የእጅ መታጠቢያና መፀዳጃ ቤቶች በእንግዳ ማረፊያ ክፍልና አዳራሽ አሉ?" },
                new EvaluationQuestion { Id = 1402, Code = "14.2", QuestionText = "የመፀዳጃ ክፍሉ ልዩ ፍላጎት ላላቸው እንግዶች ምቹ ነው?" },
                new EvaluationQuestion { Id = 1403, Code = "14.3", QuestionText = "የካፖርት ወይም ኮት መስቀያ አለ?" },
                new EvaluationQuestion { Id = 1404, Code = "14.4", QuestionText = "በኤሌክትሪክ የሚሰራ የእጅ ማድረቂያ ማሽን፣ የመጸዳጃ ወረቀት፣ ፈሳሽ ሳሙና፣ መስታወት እንዲሁም ቆሻሻ ማጠራቀሚያ አለ?" },
                new EvaluationQuestion { Id = 1405, Code = "14.5", QuestionText = "የክፍሎቹ ግድግዳ፣ ጣሪያና ወለል ይዘታቸው፣ እይታቸው በአጠቃላይ በእንግዶች ደህንነት አገልግሎትና ምቾት ጥሩ ደረጃ ላይ ነው?" }
            }
        },
        new EvaluationCategory {
            Id = 15, Name = "ሀይጅንና ንጽህና (Hygiene & Sanitation)", Icon = "fa-hand-holding-droplet",
            Questions = new List<EvaluationQuestion> {
                new EvaluationQuestion { Id = 1501, Code = "15.1", QuestionText = "ከእንግዳው ጋር ቀጥታ ግንኙነት ያላቸው የሆቴሉ ሰራተኞች በየ6 ወሩ የሚታደስ ከሚመለከተው የጤና ተቋም በንክኪና በትንፋሽ ከሚተላለፉ በሽታዎች ነፃ መሆናቸውን የሚያሳይ የጤና መርመራ ማስረጃ አላቸው?" },
                new EvaluationQuestion { Id = 1502, Code = "15.2", QuestionText = "ተገቢው የፍሳሽ ማስተላለፊያ ቱቦዎች ፍሳሹ ከዋናው መስመር ጋር የሚገናኝ ወይም ተገቢው ማጠራቀሚያ የተሰራለት ነው?" },
                new EvaluationQuestion { Id = 1503, Code = "15.3", QuestionText = "የመጠጥ ውሃ ማጣራት (Water treatment) አገልግሎት አለ?" }
            }
        },
        new EvaluationCategory {
            Id = 16, Name = "ደህንነት እና ጥበቃ (Safety & Security)", Icon = "fa-shield-halved",
            Questions = new List<EvaluationQuestion> {
                new EvaluationQuestion { Id = 1601, Code = "16.1", QuestionText = "በቂና በአግባቡ የተያዙ የአገልግሎት ጊዜያቸው ያላለፈባቸው የተለያዩ ዓይነቶች እሳት አደጋ መከላከያ መሳሪያ በየአገልግሎት መስጫ ቦታዎች አሉ?" },
                new EvaluationQuestion { Id = 1602, Code = "16.2", QuestionText = "የአደጋ ጊዜ ማስጠንቀቂያ ደወል በየአገልግሎት መስጫ ክፍሎች አለ?" },
                new EvaluationQuestion { Id = 1603, Code = "16.3", QuestionText = "ህንፃው ከሁለት ፎቆች በላይ ከሆነ የአደጋ ጊዜ መውጫ ወይም ማምለጫ አለ?" },
                new EvaluationQuestion { Id = 1604, Code = "16.4", QuestionText = "በጣም ከፍተኛ በሆነ የተደራጀ 24 ሰዓት ሁሉም የሆቴሉ ቦታዎች አገልግሎቱ ተደራሽ የሚያደርግ አስተማማኝ የደህንነት (ጥበቃ) አገልግሎት ይሰጣል?" },
                new EvaluationQuestion { Id = 1605, Code = "16.5", QuestionText = "በኮምፒውተር የታገዘ ወይም የሻንጣ መፈተሻ ማሽን አለ?" },
                new EvaluationQuestion { Id = 1606, Code = "16.6", QuestionText = "ሰዎች በውስጡ የሚተላለፉበት መፈተሻ (walk through) መሳሪያ አለ?" },
                new EvaluationQuestion { Id = 1607, Code = "16.7", QuestionText = "በተሟላ ሁኔታ የተደራጀ ዘመናዊ የደህንነት ካሜራ (Security surveillance camera) አለ?" },
                new EvaluationQuestion { Id = 1608, Code = "16.8", QuestionText = "በየፎቁ ግድግዳ የእሳት አደጋ የሸራ ውሃ መርጫ (Hose Reel) አለ?" }
            }
        },
        new EvaluationCategory {
            Id = 17, Name = "ሌሎች አገልግሎቶች (Auxiliary Service)", Icon = "fa-gear",
            Questions = new List<EvaluationQuestion> {
                new EvaluationQuestion { Id = 1701, Code = "17.1", QuestionText = "የኤሌክትሪክ ኃይል በሚቋረጥበት ጊዜ የኃይል አቅርቦት ለመስጠት የሚችል ደረጃውን የጠበቀ አውቶማቲክ ጀነሬተር (stand by generator) አለ?" },
                new EvaluationQuestion { Id = 1702, Code = "17.2", QuestionText = "ውሃ በሚጠፋበት ጊዜ ቢያንስ ለሶስት ቀናት አቅርቦት መስጠት የሚችል ንጹህና በቂ ውሃ የሚይዙ ማጠራቀሚያዎች አሉ?" }
            }
        },
        new EvaluationCategory {
            Id = 18, Name = "የሰራተኛ የሙያ ብቃት (Staff Proficiency)", Icon = "fa-user-graduate",
            Questions = new List<EvaluationQuestion> {
                new EvaluationQuestion { Id = 1801, Code = "18.1", QuestionText = "የሆቴሉ ስራ አስኪያጅ ከታወቀ ማሰልጠኛ ተቋም በሆቴል መኔጅመንት በዲግሪ የተመረቀና ቢያንስ ሁለት ዓመት የስራ ልምድ ያለው ነው?" },
                new EvaluationQuestion { Id = 1802, Code = "18.2", QuestionText = "የየስራ ክፍሉ ኃላፊዎች ከታወቀ ማሰልጠኛ ተቋም ቢያንስ በዲግሪ የተመረቁ ናቸው?" },
                new EvaluationQuestion { Id = 1803, Code = "18.3", QuestionText = "የየክፍሉ ሰራተኞች ተዛማጅ በሆነ የትምህርት መስክ ቢያንስ በዲፕሎማ የተመረቁ (ለዲፕሎማ 2 ዓመት ልምድ፣ ለዲግሪ 0 ዓመት ልምድ) ናቸው?" }
            }
        },
        new EvaluationCategory {
            Id = 19, Name = "የሰራተኞች አገልግሎት (Staff Service)", Icon = "fa-users",
            Questions = new List<EvaluationQuestion> {
                new EvaluationQuestion { Id = 1901, Code = "19.1", QuestionText = "ሁሉም ሰራተኞች ሁልጊዜ የግል ንጽህናቸውና አቋማቸው በአግባቡ ይጠብቃሉ?" },
                new EvaluationQuestion { Id = 1902, Code = "19.2", QuestionText = "የሆቴሉ የአገልግሎት ሰራተኞች የጥራት ደረጃውን የጠበቀ ተስማሚ የደንብ ልብስ አላቸው?" },
                new EvaluationQuestion { Id = 1903, Code = "19.3", QuestionText = "ሰራተኞች ማንነታቸውን የሚገልፅ በደረት ላይ የሚታይ መታወቂያ/ባጅ/ አላቸው?" },
                new EvaluationQuestion { Id = 1904, Code = "19.4", QuestionText = "ለሴትና ለወንድ የተለየ ሰራተኞች የልብስ መቀየሪያ ክፍልና ሎከር አለ?" },
                new EvaluationQuestion { Id = 1905, Code = "19.5", QuestionText = "ለሰራተኞች ለወንድና ለሴት የተለየ ሙቅና ቀዝቃዛ ውሃ፣ የእጅና መታጠቢያ፣ መፀዳጃ ክፍል፣ የንፅህና ወረቀት እንዲሁም ሳሙናና ፎጣ አለ?" },
                new EvaluationQuestion { Id = 1906, Code = "19.6", QuestionText = "ደረጃውን የጠበቀ የሰራተኞች መመገቢያ አዳራሽ አለ?" },
                new EvaluationQuestion { Id = 1907, Code = "19.7", QuestionText = "ሆቴሉ ሰራተኞች መብትና ግዴታቸውን፣ ሙያው የሚጠይቀውን የሥነምግባር እና የአስተዳደር ደንብ በዝርዝር የሚያውቁበት መረጃ (Employee Hand Book) አለ?" },
                new EvaluationQuestion { Id = 1908, Code = "19.8", QuestionText = "ግልፅ የሆነ የሰው ኃይል ቅጥር፣ የዕድገት፣ የዝውውር፣ የሥልጠና፣ ስንብት ወዘተ የሚያሳይ የአስተዳደር ደንብ (Administration Manual) አለ?" }
            }
        },
        new EvaluationCategory {
            Id = 20, Name = "ማጠቃለያ እና አስተያየቶች", Icon = "fa-clipboard-list",
            Questions = new List<EvaluationQuestion> {
                new EvaluationQuestion { Id = 2001, Code = "20", QuestionText = "ተጨማሪ አስተያየት አለ?" },
                new EvaluationQuestion { Id = 2002, Code = "21", QuestionText = "መረጃውን የሰጠው ሰርተፍኬት ጠያቂ ስምና ፊርማ ተመዝግቧል?" },
                new EvaluationQuestion { Id = 2003, Code = "22", QuestionText = "በሱፐርቫይዘሮች የተሰጠ አጠቃላይ አስተያየትና የውሳኔ ሀሳብ ተመዝግቧል?" },
                new EvaluationQuestion { Id = 2004, Code = "23", QuestionText = "መረጃውን ያረጋገጡት ሱፐርቪዥን ኦፊሰሮች ስም፣ ፊርማና ቀን ተመዝግቧል?" }
            }
        }
    };
        }
    }
}
