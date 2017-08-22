//using System;
//using SuperSocket.SocketBase;

//namespace MarquesasServer
//{
//    public static class SuperSocketAppServerInstance
//    {
//        public static MyAppServer RunningServer = new MyAppServer();
//    }

//    public class MyAppServer : AppServer
//    {
//        public override bool Start()
//        {
//            if (base.Start())
//            {
//                NewSessionConnected += new SessionHandler<AppSession>(appServer_NewSessionConnected);

//                return true;
//            }

//            return false;
//        }

//        private static void appServer_NewSessionConnected(AppSession session)
//        {
//            session.Send("Welcome to LaunchBox's SuperSocket Server!");
//        }
//    }
//}