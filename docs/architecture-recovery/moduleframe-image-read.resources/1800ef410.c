
void FUN_1800ef410(longlong param_1,int param_2,char *param_3,int param_4,int param_5,
                  longlong *param_6,longlong *param_7,undefined4 *param_8,longlong param_9,
                  ulonglong param_10)

{
  int *piVar1;
  code *pcVar2;
  char cVar3;
  int iVar4;
  undefined8 uVar5;
  longlong *plVar6;
  longlong lVar7;
  longlong *plVar8;
  longlong lVar9;
  ulonglong uVar10;
  undefined4 extraout_XMM0_Da;
  undefined1 auStack_f8 [32];
  undefined4 uStack_d8;
  int iStack_d0;
  longlong lStack_c8;
  longlong lStack_c0;
  longlong lStack_b8;
  int aiStack_a8 [2];
  int aiStack_a0 [2];
  longlong *plStack_98;
  longlong *plStack_90;
  longlong lStack_88;
  longlong lStack_80;
  undefined8 uStack_78;
  undefined1 uStack_70;
  undefined7 uStack_6f;
  undefined8 uStack_60;
  ulonglong uStack_58;
  ulonglong uStack_50;
  
  uStack_78 = 0xfffffffffffffffe;
  uStack_50 = DAT_1802a5230 ^ (ulonglong)auStack_f8;
  aiStack_a8[0] = param_4;
  aiStack_a0[0] = param_2;
  lStack_88 = param_1;
  if ((((param_2 != -1) && (param_4 != -1)) && (param_3 != (char *)0x0)) &&
     ((param_7 != (longlong *)0x0 && (param_8 != (undefined4 *)0x0)))) {
    lStack_80 = param_1 + 400;
    iVar4 = _Mtx_lock();
    if (iVar4 != 0) {
      __Throw_C_error_std__YAXH_Z(iVar4);
    }
    uStack_58 = 0xf;
    lVar7 = 0;
    uStack_60 = 0;
    uStack_70 = 0;
    lVar9 = lVar7;
    if (*param_3 != '\0') {
      lVar9 = -1;
      do {
        lVar9 = lVar9 + 1;
      } while (param_3[lVar9] != '\0');
    }
    FUN_180006300(&uStack_70,param_3,lVar9);
    uVar5 = FUN_1800f07a0(lStack_88 + 0x88,aiStack_a0);
    plVar6 = (longlong *)FUN_1800f0d10(uVar5,&uStack_70);
    if (0xf < uStack_58) {
      __3_YAXPEAX_Z(CONCAT71(uStack_6f,uStack_70));
    }
    uStack_58 = 0xf;
    uStack_60 = 0;
    uStack_70 = 0;
    plStack_98 = (longlong *)0x0;
    plStack_90 = (longlong *)0x0;
    *param_8 = (int)(plVar6[1] - *plVar6 >> 4);
    lVar9 = *plVar6;
    uVar10 = (ulonglong)param_5;
    if (uVar10 < (ulonglong)(plVar6[1] - lVar9 >> 4)) {
      plVar6 = *(longlong **)(lVar9 + 8 + uVar10 * 0x10);
      plStack_98 = *(longlong **)(lVar9 + uVar10 * 0x10);
      if (plVar6 != (longlong *)0x0) {
        LOCK();
        *(int *)(plVar6 + 1) = (int)plVar6[1] + 1;
        UNLOCK();
      }
      plStack_90 = plVar6;
      if (plStack_98 != (longlong *)0x0) {
        if (plStack_98[6] != 0) {
          lVar7 = plStack_98[6] + 0x80;
        }
        *param_6 = lVar7;
        *param_7 = plStack_98[7];
        if ((param_9 == 0) || (param_10 <= (ulonglong)plStack_98[2])) {
          cVar3 = FUN_18001dbc0(0);
          if (cVar3 != '\0') {
            uVar5 = FUN_18001db50();
            lStack_c8 = *param_7;
            iStack_d0 = aiStack_a8[0];
            uStack_d8 = 0xab;
            FUN_180015b40(uVar5,0,&DAT_18022e208,&DAT_18022e1b0);
          }
        }
        else {
          plVar8 = plStack_98;
          if (0xf < (ulonglong)plStack_98[3]) {
            plVar8 = (longlong *)*plStack_98;
          }
          sprintf_s(param_9,param_10,&DAT_18021dae0,plVar8);
          cVar3 = FUN_18001dbc0(0);
          if (cVar3 != '\0') {
            uVar5 = FUN_18001db50();
            lStack_b8 = param_9;
            lStack_c0 = *param_6;
            lStack_c8 = *param_7;
            iStack_d0 = aiStack_a8[0];
            uStack_d8 = 0xa7;
            FUN_180015b40(uVar5,0,&DAT_18022e188,&DAT_18022e1b0);
          }
        }
        plVar8 = (longlong *)FUN_1800f05e0(lStack_88 + 200,aiStack_a8);
        lVar9 = *plVar8;
        lVar7 = FUN_18001f260(extraout_XMM0_Da,lVar9,*(undefined8 *)(lVar9 + 8),&plStack_98);
        if (plVar8[1] == 0x7fffffffffffffe) {
          __Xlength_error_std__YAXPEBD_Z(&DAT_18021d318);
          pcVar2 = (code *)swi(3);
          (*pcVar2)();
          return;
        }
        plVar8[1] = plVar8[1] + 1;
        *(longlong *)(lVar9 + 8) = lVar7;
        **(longlong **)(lVar7 + 8) = lVar7;
      }
    }
    else {
      plVar6 = (longlong *)0x0;
    }
    plVar8 = plStack_90;
    if (plVar6 != (longlong *)0x0) {
      LOCK();
      plVar6 = plVar6 + 1;
      *(int *)plVar6 = (int)*plVar6 + -1;
      UNLOCK();
      if ((int)*plVar6 == 0) {
        (**(code **)*plStack_90)(plStack_90);
        LOCK();
        piVar1 = (int *)((longlong)plVar8 + 0xc);
        *piVar1 = *piVar1 + -1;
        UNLOCK();
        if (*piVar1 == 0) {
          (**(code **)(*plStack_90 + 8))();
        }
      }
    }
    iVar4 = _Mtx_unlock(lStack_80);
    if (iVar4 != 0) {
      __Throw_C_error_std__YAXH_Z(iVar4);
    }
  }
  FUN_1801e8810(uStack_50 ^ (ulonglong)auStack_f8);
  return;
}

