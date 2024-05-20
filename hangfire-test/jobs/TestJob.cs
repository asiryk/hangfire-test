namespace HangfireDemo.Jobs;

public class TestJob
{

    public void GetAsync(string path)
    {

        new HttpClient().GetAsync("http://localhost:3000/testJob/" + path);
    }
}
