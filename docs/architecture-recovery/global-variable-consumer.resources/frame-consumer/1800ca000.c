
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void FUN_1800ca000(longlong param_1,undefined4 param_2,undefined8 param_3,int param_4,
                  undefined8 param_5,int param_6,int *param_7,int *param_8)

{
  longlong *plVar1;
  int *piVar2;
  longlong lVar3;
  int iVar4;
  longlong lVar5;
  longlong *plVar6;
  undefined1 *puVar7;
  int iVar8;
  undefined1 auStack_108 [32];
  undefined8 uStack_e8;
  undefined4 auStack_e0 [2];
  longlong lStack_d8;
  longlong *plStack_d0;
  int *piStack_c8;
  int *piStack_c0;
  longlong lStack_b8;
  longlong lStack_b0;
  longlong lStack_a8;
  longlong *plStack_a0;
  undefined4 uStack_98;
  undefined8 uStack_90;
  longlong alStack_88 [2];
  undefined1 uStack_78;
  undefined7 uStack_77;
  undefined8 uStack_68;
  ulonglong uStack_60;
  ulonglong uStack_58;
  
  alStack_88[1] = 0xfffffffffffffffe;
  uStack_58 = _DAT_1802a5230 ^ (ulonglong)auStack_108;
  uStack_90 = param_5;
  piStack_c8 = param_7;
  piStack_c0 = param_8;
  iVar8 = 0;
  uStack_98 = 0;
  uStack_e8 = param_3;
  if (((param_7 != (int *)0x0) && (param_8 != (int *)0x0)) &&
     (auStack_e0[0] = param_2, FUN_18009ab20(param_1 + 8,alStack_88,auStack_e0),
     alStack_88[0] != *(longlong *)(param_1 + 8))) {
    FUN_1800cae00(param_1,&lStack_d8,param_2);
    if (lStack_d8 != 0) {
      uStack_60 = 0xf;
      uStack_68 = 0;
      uStack_78 = 0;
      FUN_180006300(&uStack_78,&UNK_18021cf35,0);
      lVar3 = *(longlong *)(lStack_d8 + 0x30);
      lVar5 = FUN_1800202d0(lStack_d8 + 0x30,uStack_e8);
      if ((lVar5 == *(longlong *)(lStack_d8 + 0x30)) ||
         (iVar4 = FUN_180020a60(uStack_e8,lVar5 + 0x20), iVar4 < 0)) {
        lStack_b0 = *(longlong *)(lStack_d8 + 0x30);
        plVar6 = &lStack_b0;
      }
      else {
        lStack_b8 = lVar5;
        plVar6 = &lStack_b8;
      }
      if (*plVar6 != lVar3) {
        FUN_1800f7260(lStack_d8,&lStack_a8,uStack_e8);
        if (lStack_a8 == 0) {
          if (plStack_a0 != (longlong *)0x0) {
            LOCK();
            plVar6 = plStack_a0 + 1;
            *(int *)plVar6 = (int)*plVar6 + -1;
            UNLOCK();
            if ((int)*plVar6 == 0) {
              (**(code **)*plStack_a0)(plStack_a0);
              LOCK();
              piVar2 = (int *)((longlong)plStack_a0 + 0xc);
              *piVar2 = *piVar2 + -1;
              UNLOCK();
              if (*piVar2 == 0) {
                (**(code **)(*plStack_a0 + 8))(plStack_a0);
              }
            }
          }
        }
        else {
          if (param_4 < 0) {
            iVar4 = -0x1fffffec;
          }
          else {
            lVar3 = *(longlong *)(lStack_a8 + 0x10);
            if (lVar3 == 0) {
              iVar4 = -0x1ffffcec;
            }
            else {
              iVar8 = *(int *)(lVar3 + 0x30);
              if (param_4 < iVar8) {
                puVar7 = *(undefined1 **)(*(longlong *)(lVar3 + 0x38) + 8 + (longlong)param_4 * 8);
                if (&uStack_78 != puVar7) {
                  FUN_180006470(&uStack_78,puVar7,0,0xffffffffffffffff);
                }
                iVar4 = 0;
              }
              else {
                iVar4 = -0x1fffffec;
                if (iVar8 == 0) {
                  iVar4 = -0x1ffffcec;
                }
              }
            }
          }
          if (plStack_a0 != (longlong *)0x0) {
            LOCK();
            plVar6 = plStack_a0 + 1;
            *(int *)plVar6 = (int)*plVar6 + -1;
            UNLOCK();
            if ((int)*plVar6 == 0) {
              (**(code **)*plStack_a0)(plStack_a0);
              LOCK();
              piVar2 = (int *)((longlong)plStack_a0 + 0xc);
              *piVar2 = *piVar2 + -1;
              UNLOCK();
              if (*piVar2 == 0) {
                (**(code **)(*plStack_a0 + 8))(plStack_a0);
              }
            }
          }
          if (iVar4 == 0) {
            *piStack_c8 = (int)uStack_68 + 1;
            if ((int)uStack_68 + 1 <= param_6) {
              puVar7 = &uStack_78;
              if (0xf < uStack_60) {
                puVar7 = (undefined1 *)CONCAT71(uStack_77,uStack_78);
              }
              sprintf_s(uStack_90,(longlong)param_6,&UNK_18021dae0,puVar7);
              *piStack_c0 = iVar8;
            }
          }
        }
      }
      if (0xf < uStack_60) {
        __3_YAXPEAX_Z(CONCAT71(uStack_77,uStack_78));
      }
      uStack_60 = 0xf;
      uStack_68 = 0;
      uStack_78 = 0;
    }
    plVar6 = plStack_d0;
    if (plStack_d0 != (longlong *)0x0) {
      LOCK();
      plVar1 = plStack_d0 + 1;
      *(int *)plVar1 = (int)*plVar1 + -1;
      UNLOCK();
      if ((int)*plVar1 == 0) {
        (**(code **)*plStack_d0)(plStack_d0);
        LOCK();
        piVar2 = (int *)((longlong)plVar6 + 0xc);
        *piVar2 = *piVar2 + -1;
        UNLOCK();
        if (*piVar2 == 0) {
          (**(code **)(*plStack_d0 + 8))();
        }
      }
    }
  }
  FUN_1801e8810(uStack_58 ^ (ulonglong)auStack_108);
  return;
}

