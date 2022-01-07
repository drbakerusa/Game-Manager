namespace GameManager.Data.F95;

public static class Map
{
    public static string GetPrefixFromId(int PrefixId) => Prefixes.GetValueOrDefault(PrefixId) ?? "unknown";

    public static List<string> GetPrefixFromId(List<int> PrefixIds)
    {
        var result = new List<string>();

        if ( PrefixIds != null && PrefixIds.Any() )
        {
            foreach ( int PrefixId in PrefixIds )
            {
                result.Add(GetPrefixFromId(PrefixId));
            }
        }
        return result;
    }

    public static string GetTagFromId(int TagId) => Tags.GetValueOrDefault(TagId) ?? "unknown";

    public static List<string> GetTagFromId(List<int> TagIds)
    {
        var result = new List<string>();

        if ( TagIds != null && TagIds.Any() )
        {
            foreach ( int TagId in TagIds )
            {
                result.Add(GetTagFromId(TagId));
            }
        }

        return result;
    }

    public static Dictionary<int, string> Prefixes = new Dictionary<int, string>()
        {
            {1,"qsp" },
            {2, "rpgm" },
            {3, "unity" },
            {4, "html" },
            {5, "rags" },
            {6,"java" },
            {7, "ren'py" },
            {8, "flash" },
            {12, "adrift" },
            {13, "vn" },
            {14, "others" },
            {17, "tads" },
            {18, "completed" },
            {19, "collection" },
            {20, "on hold" },
            {22, "abandoned" },
            {23, "site rip" },
            {30, "wolf rpg" },
            {31, "unreal engine" },
            {47, "webgl" }
        };

    public static Dictionary<int, string> Tags = new Dictionary<int, string>()
    {
        {2214, "2d game"},
        {1507,"2dcg" },
        {1434,"3d game" },
        {107, "3dcg" },
        {162,"adventure" },
        {916,"ahegao" },
        {2241,"anal sex" },
        {783,"animated" },
        {264,"bdsm" },
        {105,"bestiality" },
        {817,"big ass" },
        {130,"big tits" },
        {339,"blackmail" },
        {216,"bukkake" },
        {2247,"censored" },
        {2246,"character creation" },
        {924,"cheating" },
        {550,"combat" },
        {103,"corruption" },
        {606,"cosplay" },
        {278,"creampie" },
        {348,"dating sim" },
        {1407,"dilf" },
        {2217,"drugs" },
        {2249,"dystopian setting" },
        {384,"exhibitionism" },
        {179,"fantasy" },
        {2252,"female domination" },
        {392,"female protagonist" },
        {553,"footjob" },
        {382,"furry" },
        {191,"futa/trans" },
        {2255,"futa/trans protagonist" },
        {360,"gay" },
        {728,"graphic violence" },
        {535,"groping" },
        {498,"group sex" },
        {259,"handjob" },
        {254,"harem" },
        {708,"horror" },
        {871,"humiliation" },
        {361,"humor" },
        {30,"incest" },
        {1483,"internal view" },
        {894,"interracial" },
        {736,"japanese game" },
        {1111,"kinetic novel" },
        {290,"lactation" },
        {181,"lesbian" },
        {639,"loli" },
        {174,"male domination" },
        {173,"male protagonist" },
        {449,"management" },
        {176,"masturbation" },
        {75,"milf" },
        {111,"mind control" },
        {2229,"mobile game" },
        {182,"monster" },
        {394,"monster girl" },
        {322,"multiple endings" },
        {1556,"multiple penetration" },
        {2242,"multiple protagonist" },
        {1828,"necrophilia" },
        {324,"no sexual content" },
        {258,"ntr" },
        {237,"oral sex" },
        {408,"paranormal" },
        {505,"parody" },
        {1508,"platformer" },
        {1525,"point & click" },
        {1476,"possession" },
        {1766,"pov" },
        {225,"pregnancy" },
        {374,"prostitution" },
        {1471,"puzzle" },
        {417,"rape" },
        {1707,"real porn" },
        {2218,"religion" },
        {330,"romance" },
        {45,"rpg" },
        {2257,"sandbox" },
        {689,"scat" },
        {547,"school setting" },
        {141,"sci-fi" },
        {2216,"sex toys" },
        {670,"sexual harassment" },
        {1079,"shooter" },
        {749,"shota" },
        {776,"side-scroller" },
        {448,"simulator" },
        {2215,"sissification" },
        {44,"slave" },
        {1305,"sleep sex" },
        {769,"spanking" },
        {628,"strategy" },
        {480,"stripping" },
        {354,"superpowers" },
        {2234,"swinging" },
        {351,"teasing" },
        {215,"tentacles" },
        {522,"text based" },
        {411,"titfuck" },
        {199,"trainer" },
        {875,"transformation" },
        {362,"trap" },
        {452,"turn based combat" },
        {327,"twins" },
        {1254,"urination" },
        {2209,"vaginal sex" },
        {833,"virgin" },
        {895,"virtual reality" },
        {1506,"voiced" },
        {757,"vore" },
        {485,"voyeurism" }
    };

    public static DateTime ConvertFuzzyTime(string? fuzzyTime)
    {
        if ( string.IsNullOrEmpty(fuzzyTime) )
            return DateTime.Now;

        var now = DateTime.Now;
        if ( !string.IsNullOrEmpty(fuzzyTime) )
        {
            try
            {
                if ( fuzzyTime == "Yesterday" )
                {
                    return now.AddDays(-1);
                }

                var timeParts = fuzzyTime.Split(' ');
                int timeNumber = 0;
                string timeUnit = timeParts[1];

                if ( int.TryParse(timeParts[0], out timeNumber) )
                {
                    timeNumber *= -1;

                    switch ( timeUnit )
                    {
                        case "min":
                        case "mins":
                            return now.AddMinutes(timeNumber);
                        case "hr":
                        case "hrs":
                            return now.AddHours(timeNumber);
                        case "days":
                            return now.AddDays(timeNumber);
                        case "week":
                        case "weeks":
                            return now.AddDays(timeNumber * 7);
                        case "month":
                        case "months":
                            return now.AddMonths(timeNumber);
                        case "year":
                        case "years":
                            return now.AddYears(timeNumber);
                        default:
                            return now;
                    }
                }
            }
            catch ( Exception )
            {
                return now;
            }
        }
        return now;
    }
}
