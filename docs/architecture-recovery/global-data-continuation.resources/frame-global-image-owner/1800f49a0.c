
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void FUN_1800f49a0(longlong param_1,undefined8 *param_2,undefined4 param_3,longlong param_4,
                  longlong param_5,undefined8 param_6,undefined8 param_7,char *param_8)

{
  longlong *plVar1;
  int *piVar2;
  longlong lVar3;
  char cVar4;
  int iVar5;
  undefined8 uVar6;
  undefined8 *puVar7;
  undefined1 auStack_128 [32];
  longlong lStack_108;
  char *pcStack_100;
  undefined8 *puStack_f8;
  undefined1 *puStack_f0;
  longlong lStack_e8;
  longlong *plStack_e0;
  longlong lStack_d8;
  undefined8 uStack_d0;
  undefined1 uStack_c8;
  undefined1 auStack_c7 [127];
  ulonglong uStack_48;
  
  uStack_d0 = 0xfffffffffffffffe;
  uStack_48 = _DAT_1802a5230 ^ (ulonglong)auStack_128;
  if ((param_4 != 0) && (param_5 != 0)) {
    if (param_8 == (char *)0x0) {
      cVar4 = FUN_18001dbc0(4);
      if (cVar4 != '\0') {
        if (0xf < (ulonglong)param_2[3]) {
          param_2 = (undefined8 *)*param_2;
        }
        uVar6 = FUN_18001db50();
        pcStack_100 = (char *)CONCAT44(pcStack_100._4_4_,*(undefined4 *)(param_1 + 0x104));
        lStack_108 = CONCAT44(lStack_108._4_4_,0x5e);
        puStack_f8 = param_2;
        FUN_180015b40(uVar6,4,&UNK_18022e7e8,&UNK_18022e728);
      }
    }
    else {
      FUN_1800f79b0(param_1,&lStack_e8,param_2);
      if (lStack_e8 != 0) {
        lVar3 = param_1 + 0xf0;
        lStack_d8 = lVar3;
        iVar5 = _Mtx_lock(lVar3);
        if (iVar5 != 0) {
          __Throw_C_error_std__YAXH_Z(iVar5);
        }
        uStack_c8 = 0;
        func_0x0001801e9194(auStack_c7,0,0x7f);
        if ((*param_8 == '\0') && (cVar4 = FUN_18001dbc0(4), cVar4 != '\0')) {
          puVar7 = param_2;
          if (0xf < (ulonglong)param_2[3]) {
            puVar7 = (undefined8 *)*param_2;
          }
          uVar6 = FUN_18001db50();
          pcStack_100 = (char *)CONCAT44(pcStack_100._4_4_,*(undefined4 *)(param_1 + 0x104));
          lStack_108 = CONCAT44(lStack_108._4_4_,0x6d);
          puStack_f8 = puVar7;
          FUN_180015b40(uVar6,4,&UNK_18022e7a0,&UNK_18022e728);
        }
        pcStack_100 = param_8;
        lStack_108 = param_4;
        FUN_18009ae50(&uStack_c8,0x80,&UNK_180226ab8,param_5);
        cVar4 = FUN_18001dbc0(1);
        if (cVar4 != '\0') {
          if (0xf < (ulonglong)param_2[3]) {
            param_2 = (undefined8 *)*param_2;
          }
          uVar6 = FUN_18001db50();
          puStack_f0 = &uStack_c8;
          pcStack_100 = (char *)CONCAT44(pcStack_100._4_4_,*(undefined4 *)(param_1 + 0x104));
          lStack_108 = CONCAT44(lStack_108._4_4_,0x72);
          puStack_f8 = param_2;
          FUN_180015b40(uVar6,1,&UNK_18022e850,&UNK_18022e728);
        }
        FUN_1800f41e0(lStack_e8,param_3,&uStack_c8);
        iVar5 = _Mtx_unlock(lVar3);
        if (iVar5 != 0) {
          __Throw_C_error_std__YAXH_Z(iVar5);
        }
      }
      if (plStack_e0 != (longlong *)0x0) {
        LOCK();
        plVar1 = plStack_e0 + 1;
        *(int *)plVar1 = (int)*plVar1 + -1;
        UNLOCK();
        if ((int)*plVar1 == 0) {
          (**(code **)*plStack_e0)(plStack_e0);
          LOCK();
          piVar2 = (int *)((longlong)plStack_e0 + 0xc);
          *piVar2 = *piVar2 + -1;
          UNLOCK();
          if (*piVar2 == 0) {
            (**(code **)(*plStack_e0 + 8))(plStack_e0);
          }
        }
      }
    }
  }
  FUN_1801e8810(uStack_48 ^ (ulonglong)auStack_128);
  return;
}

