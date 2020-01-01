using System;
using System.Collections.Generic;
using System.Text;

namespace DAL
{
    public class CosmosDBSettings
    {

        public static readonly string ENDPOINT_URI = "https://ms-ng-nosql.documents.azure.com:443/";

        public static readonly string KEY = "0LdqjvoMM7DKOn1XYqs2RhaIH42RSV8E6K5TfuoafEPMb9b60vA4weVXUtUKNareYdxGy91fv1AOUP6wpCn2Cw==";

        public static readonly string DB_ID = "ChatDatabase";
        public static readonly string GROUP_CONTAINER_ID = "GroupContainer";
        public static readonly string USER_CONTAINER_ID = "UserContainer";
        public static readonly string MESSAGE_CONTAINER_ID = "MsgContainer";
    }
}
