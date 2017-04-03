using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FRL.IO {
  public interface IDaydreamHandler : IPointerDaydreamHandler, IGlobalDaydreamHandler { }
  public interface IPointerDaydreamHandler : IPointerAppMenuHandler, IPointerTouchpadHandler { }
  public interface IGlobalDaydreamHandler : IGlobalApplicationMenuHandler, IGlobalTouchpadHandler { }
}

