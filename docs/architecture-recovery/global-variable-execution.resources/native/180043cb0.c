
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

undefined4 FUN_180043cb0(longlong param_1,int param_2)

{
  int *piVar1;
  longlong *plVar2;
  longlong *plVar3;
  longlong *plVar4;
  char cVar5;
  int iVar6;
  undefined4 uVar7;
  undefined8 uVar8;
  longlong *plVar9;
  longlong *plVar10;
  longlong *plStack_50;
  undefined8 uStack_48;
  longlong lStack_40;
  longlong *plStack_38;
  longlong lStack_30;
  longlong *plStack_28;
  
  uVar7 = 0;
  cVar5 = FUN_1800441c0(param_1,3000);
  if (cVar5 == '\0') {
    uVar7 = 0xe000001e;
  }
  else {
    if ((*(longlong *)(param_1 + 0x550) != 0) && (iVar6 = SetEvent(), iVar6 == 0)) {
      GetLastError();
    }
    if (param_2 == 10999) {
      uStack_48 = 0;
      plStack_50 = (longlong *)FUN_18001f150();
      if ((_DAT_1802b1888 & 1) == 0) {
        _DAT_1802b1888 = _DAT_1802b1888 | 1;
        FUN_180107fc0();
        FUN_1801e8930(&UNK_180202980);
      }
      iVar6 = FUN_180115330(&DAT_1802b1890,&plStack_50);
      if ((iVar6 == 0) && (plVar9 = (longlong *)*plStack_50, plVar9 != plStack_50)) {
        do {
          plVar10 = (longlong *)plVar9[3];
          lStack_30 = plVar9[2];
          if (plVar10 != (longlong *)0x0) {
            LOCK();
            *(int *)(plVar10 + 1) = (int)plVar10[1] + 1;
            UNLOCK();
          }
          plStack_28 = plVar10;
          if (((lStack_30 != 0) && (*(int *)(lStack_30 + 0x50) != 10999)) &&
             (*(int *)(lStack_30 + 0x50) != 100000)) {
            FUN_1800dd030(lStack_30,0);
          }
          if (plVar10 != (longlong *)0x0) {
            LOCK();
            plVar3 = plVar10 + 1;
            *(int *)plVar3 = (int)*plVar3 + -1;
            UNLOCK();
            if ((int)*plVar3 == 0) {
              (**(code **)*plVar10)(plVar10);
              LOCK();
              piVar1 = (int *)((longlong)plVar10 + 0xc);
              *piVar1 = *piVar1 + -1;
              UNLOCK();
              if (*piVar1 == 0) {
                (**(code **)(*plVar10 + 8))(plVar10);
              }
            }
          }
          plVar9 = (longlong *)*plVar9;
        } while (plVar9 != plStack_50);
      }
      if ((*(int *)(param_1 + 0x540) != 0 && *(longlong *)(param_1 + 0x548) != 0) &&
         (iVar6 = SetEvent(), iVar6 == 0)) {
        GetLastError();
      }
      plVar9 = (longlong *)*plStack_50;
      *plStack_50 = (longlong)plStack_50;
      plStack_50[1] = (longlong)plStack_50;
      plVar10 = plStack_50;
      if (plVar9 != plStack_50) {
        do {
          plVar3 = (longlong *)*plVar9;
          plVar4 = (longlong *)plVar9[3];
          if (plVar4 != (longlong *)0x0) {
            LOCK();
            plVar2 = plVar4 + 1;
            *(int *)plVar2 = (int)*plVar2 + -1;
            UNLOCK();
            plVar10 = plStack_50;
            if ((int)*plVar2 == 0) {
              (**(code **)*plVar4)(plVar4);
              LOCK();
              piVar1 = (int *)((longlong)plVar4 + 0xc);
              *piVar1 = *piVar1 + -1;
              UNLOCK();
              plVar10 = plStack_50;
              if (*piVar1 == 0) {
                (**(code **)(*plVar4 + 8))(plVar4);
                plVar10 = plStack_50;
              }
            }
          }
          __3_YAXPEAX_Z(plVar9);
          plVar9 = plVar3;
        } while (plVar3 != plVar10);
      }
      __3_YAXPEAX_Z(plVar10);
    }
    else {
      if ((_DAT_1802b1888 & 1) == 0) {
        _DAT_1802b1888 = _DAT_1802b1888 | 1;
        FUN_180107fc0();
        FUN_1801e8930(&UNK_180202980);
      }
      FUN_180108ba0(&DAT_1802b1890,&lStack_40,param_2);
      if (lStack_40 == 0) {
        cVar5 = FUN_18001dbc0(4);
        if (cVar5 != '\0') {
          uVar8 = FUN_18001db50();
          FUN_180015b40(uVar8,4,&UNK_18021f760,&UNK_18021f730,0x40f,param_2);
        }
        uVar7 = 0xe0000316;
      }
      else {
        uVar7 = FUN_1800dd030(lStack_40,0);
      }
      plVar9 = plStack_38;
      if (plStack_38 != (longlong *)0x0) {
        LOCK();
        plVar10 = plStack_38 + 1;
        *(int *)plVar10 = (int)*plVar10 + -1;
        UNLOCK();
        if ((int)*plVar10 == 0) {
          (**(code **)*plStack_38)(plStack_38);
          LOCK();
          piVar1 = (int *)((longlong)plVar9 + 0xc);
          *piVar1 = *piVar1 + -1;
          UNLOCK();
          if (*piVar1 == 0) {
            (**(code **)(*plStack_38 + 8))();
          }
        }
      }
    }
  }
  return uVar7;
}

