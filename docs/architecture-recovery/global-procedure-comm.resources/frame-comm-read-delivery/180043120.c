
ulonglong FUN_180043120(longlong param_1,undefined4 param_2,longlong param_3,undefined8 param_4,
                       undefined4 param_5)

{
  longlong *plVar1;
  int *piVar2;
  longlong *plVar3;
  char cVar4;
  int iVar5;
  uint uVar6;
  longlong lVar7;
  undefined8 uVar8;
  ulonglong uVar9;
  undefined8 in_stack_ffffffffffffffb0;
  undefined4 uVar10;
  undefined8 uVar11;
  longlong lStack_40;
  longlong *plStack_38;
  longlong lStack_30;
  longlong *plStack_28;
  longlong lStack_20;
  longlong *plStack_18;
  
  uVar10 = (undefined4)((ulonglong)in_stack_ffffffffffffffb0 >> 0x20);
  uVar11 = 0xfffffffffffffffe;
  if (param_3 != 0) {
    lVar7 = -1;
    do {
      lVar7 = lVar7 + 1;
    } while (*(char *)(param_3 + lVar7) != '\0');
    if (lVar7 != 0) {
      if (*(char *)(param_1 + 0x22) != '\0') {
        cVar4 = FUN_18001dbc0(4);
        if (cVar4 != '\0') {
          uVar8 = FUN_18001db50();
          FUN_180015b40(uVar8,4,&UNK_18021f4f8,&UNK_18021f4d8,0x315,param_3,uVar11);
        }
        return 0xe000001d;
      }
      iVar5 = _stricmp(param_3,&UNK_18021f538);
      if (iVar5 == 0) {
        uVar9 = FUN_18005d3f0();
        return uVar9;
      }
      iVar5 = _stricmp(param_3,&UNK_18021f550);
      if ((iVar5 == 0) || (iVar5 = _stricmp(param_3,&UNK_18021f560), iVar5 == 0)) {
        uVar11 = FUN_18008e5d0();
        cVar4 = FUN_180110680(uVar11,param_2);
        if (cVar4 != '\0') {
          uVar11 = FUN_18008e5d0();
          FUN_1801111d0(uVar11,&lStack_40,param_2);
          plVar3 = plStack_38;
          if (lStack_40 != 0) {
            uVar6 = FUN_1800a3200(lStack_40,param_3,param_4,param_5);
            plVar3 = plStack_38;
            uVar9 = (ulonglong)uVar6;
            if (plStack_38 == (longlong *)0x0) {
              return uVar9;
            }
            LOCK();
            plVar1 = plStack_38 + 1;
            *(int *)plVar1 = (int)*plVar1 + -1;
            UNLOCK();
            if ((int)*plVar1 != 0) {
              return uVar9;
            }
            (**(code **)*plStack_38)(plStack_38);
            LOCK();
            piVar2 = (int *)((longlong)plVar3 + 0xc);
            *piVar2 = *piVar2 + -1;
            UNLOCK();
            if (*piVar2 != 0) {
              return uVar9;
            }
            (**(code **)(*plStack_38 + 8))();
            return uVar9;
          }
          if (plStack_38 != (longlong *)0x0) {
            LOCK();
            plVar1 = plStack_38 + 1;
            *(int *)plVar1 = (int)*plVar1 + -1;
            UNLOCK();
            if ((int)*plVar1 == 0) {
              (**(code **)*plStack_38)(plStack_38);
              LOCK();
              piVar2 = (int *)((longlong)plVar3 + 0xc);
              *piVar2 = *piVar2 + -1;
              UNLOCK();
              if (*piVar2 == 0) {
                (**(code **)(*plStack_38 + 8))();
              }
            }
          }
        }
        uVar11 = FUN_18008e5d0();
        cVar4 = FUN_180108b20(uVar11,param_2);
        if (cVar4 != '\0') {
          uVar11 = FUN_18008e5d0();
          FUN_180108ba0(uVar11,&lStack_30,param_2);
          plVar3 = plStack_28;
          if (lStack_30 != 0) {
            uVar6 = FUN_1800e1d20(lStack_30,param_3,param_4,param_5);
            plVar3 = plStack_28;
            uVar9 = (ulonglong)uVar6;
            if (plStack_28 == (longlong *)0x0) {
              return uVar9;
            }
            LOCK();
            plVar1 = plStack_28 + 1;
            *(int *)plVar1 = (int)*plVar1 + -1;
            UNLOCK();
            if ((int)*plVar1 != 0) {
              return uVar9;
            }
            (**(code **)*plStack_28)(plStack_28);
            LOCK();
            piVar2 = (int *)((longlong)plVar3 + 0xc);
            *piVar2 = *piVar2 + -1;
            UNLOCK();
            if (*piVar2 != 0) {
              return uVar9;
            }
            (**(code **)(*plStack_28 + 8))();
            return uVar9;
          }
          if (plStack_28 != (longlong *)0x0) {
            LOCK();
            plVar1 = plStack_28 + 1;
            *(int *)plVar1 = (int)*plVar1 + -1;
            UNLOCK();
            if ((int)*plVar1 == 0) {
              (**(code **)*plStack_28)(plStack_28);
              LOCK();
              piVar2 = (int *)((longlong)plVar3 + 0xc);
              *piVar2 = *piVar2 + -1;
              UNLOCK();
              if (*piVar2 == 0) {
                (**(code **)(*plStack_28 + 8))();
              }
            }
          }
        }
      }
      uVar11 = FUN_18008e5d0();
      FUN_18010a690(uVar11,&lStack_20,param_2);
      if (lStack_20 == 0) {
        cVar4 = FUN_18001dbc0(4);
        if (cVar4 != '\0') {
          uVar11 = FUN_18001db50();
          FUN_180015b40(uVar11,4,&UNK_18021ef30,&UNK_18021f4d8,0x336,CONCAT44(uVar10,param_2));
        }
        uVar9 = 0xe0000302;
      }
      else {
        plVar3 = *(longlong **)(lStack_20 + 0x128);
        if (plVar3 == (longlong *)0x0) {
          cVar4 = FUN_18001dbc0(4);
          if (cVar4 != '\0') {
            uVar11 = FUN_18001db50();
            FUN_180015b40(uVar11,4,&UNK_18021f570,&UNK_18021f4d8,0x33f);
          }
          uVar6 = FUN_1800b9390(lStack_20,param_3,param_4,param_5);
        }
        else {
          uVar6 = (**(code **)(*plVar3 + 0x18))(plVar3,param_3,param_4,param_5);
        }
        uVar9 = (ulonglong)uVar6;
      }
      plVar3 = plStack_18;
      if (plStack_18 == (longlong *)0x0) {
        return uVar9;
      }
      LOCK();
      plVar1 = plStack_18 + 1;
      *(int *)plVar1 = (int)*plVar1 + -1;
      UNLOCK();
      if ((int)*plVar1 != 0) {
        return uVar9;
      }
      (**(code **)*plStack_18)(plStack_18);
      LOCK();
      piVar2 = (int *)((longlong)plVar3 + 0xc);
      *piVar2 = *piVar2 + -1;
      UNLOCK();
      if (*piVar2 != 0) {
        return uVar9;
      }
      (**(code **)(*plStack_18 + 8))();
      return uVar9;
    }
  }
  cVar4 = FUN_18001dbc0(4);
  if (cVar4 != '\0') {
    uVar11 = FUN_18001db50();
    FUN_180015b40(uVar11,4,&UNK_18021ef90,&UNK_18021f4d8,0x30f);
  }
  return 0xe0000001;
}

