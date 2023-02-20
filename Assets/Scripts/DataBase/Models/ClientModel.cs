using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientModel : DataBaseModel
{
    public IntegerField idx;
    public TextField name;
    public IntegerField iconId;
    public IntegerField isConnected;
    public ClientModel() : base("clients")
    {
        idx = new IntegerField(this, "idx", true);
        name = new TextField(this, "name");
        iconId = new IntegerField(this, "icon_id");
        isConnected = new IntegerField(this, "is_connected");
    }

    public override DataBaseModel Clone()
    {
        return new ClientModel();
    }
}
