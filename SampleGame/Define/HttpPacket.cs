namespace SampleGame.Define
{
    /* 유니티 클라이언트 프로젝트 공유용 파일 */

    public class ResBase
    {
        public int code { get; set; }
    }

    public class ResDebug : ResBase
    {
        public string debug { get; set; }
    }

    public class ReqCommon
    {
        public string token { get; set; }
    }

    public class ReqPlayerLogin
    {
        public string key { get; set; }
    }

    public class ResPlayerLogin : ResBase
    {
        public string token { get; set; }
        public long playerid { get; set; }
        public string name { get; set; }
    }

    public class ReqPlayerUpdateName : ReqCommon
    {
        public string name { get; set; }
    }

    public class ResPlayerUpdateName : ResBase
    {
    }
}