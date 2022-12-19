using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public abstract class AbstractAbility : MonoBehaviour
{
    int Cooldown {get;}
    protected abstract void Activate();
    protected abstract void Deactivate();
}
