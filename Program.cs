
namespace JarvisChat
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var jarvis = new Jarvis();
            await jarvis.RunAsync();
        }
    }

}
