using System.Collections.Generic;

namespace MarquesasServer
{
    public class GameObject
    {
        public string Title;
        public string Marque;
    }

    public static class MarquesasHttpServerInstance
    {
        public static MarquesasHttpServer RunningServer = new MarquesasHttpServer();
    }

}
