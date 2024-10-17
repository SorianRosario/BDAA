public class Klkmdw
{
    private readonly RequestDelegate _next;

    public Klkmdw(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        Console.WriteLine("===>PUDE HACERLO SIN EL CHATGPT<===");
        await _next(context);
    }
}
