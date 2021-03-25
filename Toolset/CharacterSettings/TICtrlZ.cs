using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TICtrlZ : ToolItem
{
    static Stack<HistoryItem> history = new Stack<HistoryItem>();
    public static Stack<HistoryItem> History { get { return history; } }

    static int lastID = -1;
    public static int LastID { get { return lastID; } set { lastID = value; } }

    public override void ActivateItem()
    {
        if (!BoltNetwork.IsRunning || BoltNetwork.IsServer)
        {
            if (history.Count > 0)
                history.Pop().CtrlZ();
            
        }
        else if (BoltNetwork.IsClient)
        {
            BlockCtrlZ.Create().Send();
        }
    }

    public override void FocusItem()
    {
        Highlight();
    }

    public override void UnfocusItem()
    {
        base.UnfocusItem();

        Lowlight();
    }

}

public class HistoryItem
{
    bool delete = false;

    int addedID;

    Part deletedPart;

    Vector3 position;

    Quaternion rotation;

    public HistoryItem(int added)
    {
        addedID = added;
    }

    public HistoryItem(Part deleted, Vector3 pos, Quaternion rot)
    {
        delete = true;
        deletedPart = deleted;
        position = pos;
        rotation = rot;
    }

    public void CtrlZ()
    {
        if (delete)
        {
            TICtrlZ.LastID = deletedPart.ID;

            if (!BoltNetwork.IsRunning)
            {
                GameObject go = PartsHolder.Holder.SpawnGhostPart(deletedPart.TemplateID);

                go.transform.position = position;
                go.transform.rotation = rotation;
                Part part = go.GetComponent<Part>();
                part.FreezePart(deletedPart.ID);
                part.Parent = deletedPart.Parent;
                part.ParentCon = deletedPart.ParentCon;
                try
                {
                    GlobalReferences.FrozenParts[part.Parent].GetComponent<Part>().SetInactive(part.ParentCon);
                }
                catch { Debug.LogError("Parent not found for Part: " + part.ID); }

                part.Children = deletedPart.Children;
                part.ChildCons = deletedPart.ChildCons;
                for (int childID = 0; childID < part.Children.Count; ++childID)
                    try
                    {
                        GlobalReferences.FrozenParts[part.Children[childID]].GetComponent<Part>().SetInactive(part.ChildCons[childID]);
                    }
                    catch { Debug.LogError("Child not found for Part: " + part.ID); }

                part.PartOwner = deletedPart.PartOwner;
                part.Respawn = deletedPart.Respawn;

                foreach (int i in deletedPart.ActiveConnections)
                {
                    part.SetInactive(i);
                }

                
            }
            else
            {
                var spawn = SpawnFrozenBlock.Create();
                spawn.Position = position;
                spawn.Rotation = rotation;
                spawn.Owner = deletedPart.PartOwner;
                spawn.token = new PartTokenParent(deletedPart);
                spawn.Send();
            }

            
        }
        else
        {
            TICtrlZ.LastID = addedID;


            Part delPart = GlobalReferences.FrozenParts[addedID].GetComponent<Part>();

            if (delPart != null)
                delPart.Delete();
            else
                Debug.LogError("Part " + addedID + " not found - cant be undone");

            
        }
    }
}
