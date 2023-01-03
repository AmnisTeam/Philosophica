using System.Net.Http;
using System.Text;
using Newtonsoft.Json.Linq;

public class ServerUtils {
    const string API_URL_BASE = "http://localhost:5000/";
    const string API_URL_GET_NICKNAME = API_URL_BASE+"nickname";
    const string API_URL_CREATE_GAME = API_URL_BASE+"create_game";
    const string API_URL_CHECK_JOIN = API_URL_BASE+"check";
    const string API_URL_START_GAME = API_URL_BASE+"start";

    // Здесь будет посылаться POST с UUID игрока для получения его ника.
    public static string getNickname(string player_uuid) {
        JObject payload = new JObject(new JProperty("player_uuid", player_uuid));

        var client = new HttpClient();
        var content = new StringContent(payload.ToString(), Encoding.UTF8, "application/json");
        var response = client.PostAsync(API_URL_GET_NICKNAME, content).Result;
        response.EnsureSuccessStatusCode();

        string responseBody = response.Content.ReadAsStringAsync().Result;
        JObject result = JObject.Parse(responseBody);
        return result.GetValue("player_name").ToString();
    }

    // Здесь будет посылаться POST с типом игры (здесь всегда 1, мы же не хотим все усложнять :) ), выбиранной картой и UUID игрока для получения ID лобби.
    public int createGame(int game_type, int selected_map, string player_uuid) {
        return 1;
    }

    // Здесь будет посылаться POST с ID лобби для того, чтобы узнать о возможности зайти в лобби.
    /* Возвращает true, если:
         1) лобби впринципе существует;
         2) игроков в лобби меньше 4;
         3) игра еще не началась.
       Иначе возращает false.
    */
    public bool checkIfCanJoin(int lobby_id) {
        return true;
    }

    // Здесь будет посылаться POST с ID лобби для его закрытия и запуска игры.
    public void startGame(int lobby_id) {
        // TODO
    }

}
