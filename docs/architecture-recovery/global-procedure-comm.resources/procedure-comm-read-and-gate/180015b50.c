
undefined8 * FUN_180015b50(longlong param_1,undefined8 *param_2,int param_3)

{
  longlong *plVar1;
  undefined8 *puVar2;
  int *piVar3;
  longlong lVar4;
  longlong *plVar5;
  int iVar6;
  
  iVar6 = _Mtx_lock(param_1 + 0x70);
  if (iVar6 != 0) {
    __Throw_C_error_std__YAXH_Z(iVar6);
  }
  *param_2 = 0;
  param_2[1] = 0;
  puVar2 = (undefined8 *)**(undefined8 **)(param_1 + 0x60);
  do {
    if (puVar2 == *(undefined8 **)(param_1 + 0x60)) {
LAB_180015c02:
      iVar6 = _Mtx_unlock(param_1 + 0x70);
      if (iVar6 != 0) {
        __Throw_C_error_std__YAXH_Z(iVar6);
      }
      return param_2;
    }
    piVar3 = (int *)puVar2[2];
    if (*piVar3 == param_3) {
      lVar4 = puVar2[3];
      if (lVar4 != 0) {
        LOCK();
        *(int *)(lVar4 + 8) = *(int *)(lVar4 + 8) + 1;
        UNLOCK();
      }
      plVar5 = (longlong *)param_2[1];
      param_2[1] = lVar4;
      *param_2 = piVar3;
      if (plVar5 != (longlong *)0x0) {
        LOCK();
        plVar1 = plVar5 + 1;
        *(int *)plVar1 = (int)*plVar1 + -1;
        UNLOCK();
        if ((int)*plVar1 == 0) {
          (**(code **)*plVar5)(plVar5);
          LOCK();
          piVar3 = (int *)((longlong)plVar5 + 0xc);
          *piVar3 = *piVar3 + -1;
          UNLOCK();
          if (*piVar3 == 0) {
            (**(code **)(*plVar5 + 8))();
          }
        }
      }
      goto LAB_180015c02;
    }
    puVar2 = (undefined8 *)*puVar2;
  } while( true );
}

