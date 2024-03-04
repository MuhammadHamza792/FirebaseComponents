using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Project.Scripts.HelperClasses
{
    public static class Helper
    {
        private static Dictionary<float, WaitForSeconds> _cachedWaitForSeconds;
        private static Dictionary<float, WaitForSecondsRealtime> _cachedWaitForSecondsRealtime;
        private static readonly Dictionary<double, string> Prefixes = new()
        {
            {3,"K"},
            {6,"M"},
            {9,"B"},
            {12,"T"},
            {15,"aa"},
            {18,"ab"},
            {21,"ac"},
            {24,"ad"},
            {27,"ae"},
            {30,"af"},
            {33,"ag"},
            {36,"ah"},
            {39,"ai"},
            {42,"aj"},
            {45,"ak"},
            {48,"al"},
            {51,"am"},
            {54,"an"},
            {57,"ao"},
            {60,"ap"},
            {63,"aq"},
            {66,"ar"},
            {69,"as"},
            {72,"at"},
            {75,"au"},
            {78,"av"},
            {81,"aw"},
            {84,"ax"},
            {87,"ay"},
            {90,"az"},
            {93,"ba"},
            {96,"bb"},
            {99,"bc"},
            
            {102,"bd"},
            {105,"be"},
            {108,"bf"},
            {111,"bg"},
            {114,"bh"},
            {117,"bi"},
            {120,"bj"},
            {123,"bk"},
            {126,"bl"},
            {129,"bm"},
            {132,"bn"},
            {135,"bo"},
            {138,"bp"},
            {141,"bq"},
            {144,"br"},
            {147,"bs"},
            {150,"bt"},
            {153,"bu"},
            {156,"bv"},
            {159,"bw"},
            {162,"bx"},
            {165,"by"},
            {168,"bz"},
            {171,"ca"},
            {174,"cb"},
            {177,"cc"},
            {180,"cd"},
            {183,"ce"},
            {186,"cf"},
            {189,"cg"},
            {192,"ch"},
            {195,"ci"},
            {198,"cj"},
            
            {201,"ck"},
            {204,"cl"},
            {207,"cm"},
            {210,"cn"},
            {213,"co"},
            {216,"cp"},
            {219,"cq"},
            {222,"cr"},
            {225,"cs"},
            {228,"ct"},
            {231,"cu"},
            {234,"cv"},
            {237,"cw"},
            {240,"cx"},
            {243,"cy"},
            {246,"cz"},
            {249,"da"},
            {252,"db"},
            {255,"dc"},
            {258,"dd"},
            {261,"de"},
            {264,"df"},
            {267,"dg"},
            {270,"dh"},
            {273,"di"},
            {276,"dj"},
            {279,"dk"},
            {282,"dl"},
            {285,"dm"},
            {288,"dn"},
            {291,"do"},
            {294,"dp"},
            {297,"dq"},
            {300,"dr"},
            
            {303,"ds"},
        };

        public static string NotateNumber(double number, float notateAfter)
        {
            var exp = Math.Floor(Math.Log10(number));
            var thirdExp =  3 * Math.Floor(exp / 3);

            if (number >= MathHelper.Pow(10, notateAfter))
                return (number / Math.Pow(10, thirdExp)).ToString("F2") + Prefixes[thirdExp];
            return number.ToString("F2");
        }

        public static WaitForSeconds GetCachedWaitForSeconds(float seconds)
        {
            _cachedWaitForSeconds ??= new Dictionary<float, WaitForSeconds>();
            if (_cachedWaitForSeconds.TryGetValue(seconds, out var cachedSeconds)) return cachedSeconds;
            var newSeconds = new WaitForSeconds(seconds);
            _cachedWaitForSeconds.Add(seconds, newSeconds);
            return newSeconds;
        }

        public static WaitForSecondsRealtime GetCachedWaitForSecondsRealtime(float seconds)
        {
            _cachedWaitForSecondsRealtime ??= new Dictionary<float, WaitForSecondsRealtime>();
            if (_cachedWaitForSecondsRealtime.TryGetValue(seconds, out var cachedSeconds)) return cachedSeconds;
            var newSeconds = new WaitForSecondsRealtime(seconds);
            _cachedWaitForSecondsRealtime.Add(seconds, newSeconds);
            return newSeconds;
        }
        
        public static T GetOrAdd<T>(this GameObject gameObject) where T : Component
        {
            var comp = gameObject.GetComponent<T>();
            if (comp != null) return comp;
            comp = gameObject.AddComponent<T>();
            return comp;
        }
        
        public static Vector3 GetWorldPos(this Vector3 mousePosition, Camera camera)
        {
            var worldPos = camera.ScreenToWorldPoint(mousePosition);
            return worldPos;
        }
    }
}