using UnityEngine;

public class TeleportMod : Mod
{
    bool hasTeleported = false;
    public float teleportDistance = 5f;
    Vector3 lastLocation = Vector3.zero;
    Network_Player player;

    static TeleportMod instance;

    public void Start()
    {
        instance = this;
        player = RAPI.GetLocalPlayer();
        Debug.Log("Click 'K' to teleport, 'L' to teleport back.");
    }

    public void OnModUnload()
    {
        Debug.Log("Mod TeleportMod has been unloaded!");
    }

    public void Update()
    {
        if (!RAPI.IsCurrentSceneGame()) return;
        if (RConsole.isOpen) return;
        if (ChatTextFieldController.IsChatWindowSelected) return;

        RaycastHit hit;

        if (Input.GetKeyDown(KeyCode.L))
        {
            if (hasTeleported)
            {
                player.transform.position = lastLocation;
                Debug.Log("Teleported back to last location: " + lastLocation);
            }
            else
            {
                Debug.Log("Cannot teleport back; no previous location stored.");
            }
        }
        else if (Input.GetKeyDown(KeyCode.K))
        {
            if (teleportDistance <= 0f)
            {
                if (Physics.Raycast(player.transform.position, Camera.main.transform.forward, out hit, 10000f, LayerMask.GetMask("Obstruction", "Tree", "Block", "Water")))
                {
                    lastLocation = player.transform.position;
                    player.transform.position = hit.point + Vector3.up * 1f;
                    hasTeleported = true;
                    Debug.Log("Teleported to new location: " + hit.point);
                }
                else
                {
                    Debug.Log("No valid teleport location found.");
                }
            }
            else
            {
                lastLocation = player.transform.position;
                player.transform.position += Camera.main.transform.forward * teleportDistance;
                hasTeleported = true;
                Debug.Log("Teleported forward to: " + player.transform.position);
            }
        }
    }

    [ConsoleCommand(name: "teleportdistance",
        docs: "Sets the distance for teleportation.\n" +
              "Usage: teleportdistance <distance>\n" +
              "Parameters:\n" +
              "<distance>: A float value representing the teleport distance.\n" +
              "    - If set to 0, teleportation will occur to the object you're currently looking at.\n" +
              "    - If set to a positive value, you will teleport forward by that distance.\n" +
              "    - If a negative value is provided, the teleport distance will be reset to 0.\n" +
              "Example:\n" +
              "    teleportdistance 5.0  // Sets the teleport distance to 5 units forward.\n" +
              "    teleportdistance 0      // Teleports to the object you are looking at.")]
    public static void teleportdistance(string[] arg)
    {
        if (arg.Length > 0 && float.TryParse(arg[0], out float x))
        {
            if (x >= 0)
            {
                instance.teleportDistance = x;
                Debug.Log("Teleport distance set to: " + x);
            }
            else
            {
                instance.teleportDistance = 0f;
                Debug.Log("Teleport distance reset to 0.");
            }
        }
        else
        {
            Debug.Log("Invalid input for teleport distance. Please enter a valid number.");
        }
    }
}
