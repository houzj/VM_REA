
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

ulonglong FUN_1800dcc80(longlong param_1,char param_2,int param_3,longlong param_4,
                       undefined8 param_5,undefined1 param_6)

{
  longlong *plVar1;
  int *piVar2;
  code *pcVar3;
  longlong *plVar4;
  char cVar5;
  int iVar6;
  uint uVar7;
  undefined8 uVar8;
  undefined8 *puVar9;
  ulonglong uVar10;
  undefined8 in_stack_ffffffffffffff58;
  undefined4 uVar11;
  uint uStack_98;
  undefined8 uStack_80;
  undefined8 uStack_78;
  undefined1 auStack_70 [24];
  undefined8 *puStack_58;
  undefined1 auStack_50 [24];
  undefined8 *puStack_38;
  
  uVar11 = (undefined4)((ulonglong)in_stack_ffffffffffffff58 >> 0x20);
  iVar6 = _Mtx_lock(param_1 + 0x100);
  if (iVar6 != 0) {
    __Throw_C_error_std__YAXH_Z(iVar6);
  }
  if ((_DAT_1802a56bc < 2) || (*(char *)(param_1 + 0xe8) == '\0')) {
    if (*(char *)(param_1 + 0x7c) == '\0') {
      if (_DAT_1802a5690 != 0) {
        cVar5 = FUN_18001dbc0(4);
        if (cVar5 != '\0') {
          uVar8 = FUN_18001db50();
          FUN_180015b40(uVar8,4,&UNK_18022c920,&UNK_18022c968,CONCAT44(uVar11,0xb1));
        }
        uVar10 = 0xe0000700;
        goto LAB_1800dd007;
      }
      cVar5 = FUN_1800dd200(param_1);
      if (cVar5 != '\0') {
        uVar10 = 0xe0000300;
        if ((DAT_1802b129e == '\0') || (*(char *)(param_1 + 400) == '\0')) {
          uVar8 = FUN_18008ea20();
          FUN_1800d8d30(uVar8,*(undefined4 *)(param_1 + 0x50));
        }
        else {
          uStack_80 = (code *)0x0;
          uStack_78 = (longlong *)0x0;
          uVar8 = FUN_18008ede0();
          uVar7 = FUN_1800e86c0(uVar8,*(undefined4 *)(param_1 + 0x50),&uStack_80);
          if (uVar7 == 0) {
            *(undefined8 *)(uStack_80 + 0x1a0) = param_5;
            uVar7 = FUN_1800dcc80(uStack_80,param_2,param_3,param_4,param_6,0);
          }
          plVar4 = uStack_78;
          uVar10 = (ulonglong)uVar7;
          if (uStack_78 != (longlong *)0x0) {
            LOCK();
            plVar1 = uStack_78 + 1;
            *(int *)plVar1 = (int)*plVar1 + -1;
            UNLOCK();
            if ((int)*plVar1 == 0) {
              (**(code **)*uStack_78)(uStack_78);
              LOCK();
              piVar2 = (int *)((longlong)plVar4 + 0xc);
              *piVar2 = *piVar2 + -1;
              UNLOCK();
              if (*piVar2 == 0) {
                (**(code **)(*uStack_78 + 8))();
              }
            }
          }
        }
        goto LAB_1800dd007;
      }
      uVar7 = FUN_1800de620(param_1);
      if (uVar7 != 0) {
        uVar10 = (ulonglong)uVar7;
        goto LAB_1800dd007;
      }
      uVar8 = FUN_18008e590();
      uVar7 = FUN_1800580c0(uVar8);
      uVar10 = (ulonglong)uVar7;
      if (uVar7 != 0) goto LAB_1800dd007;
      *(undefined1 *)(param_1 + 0xe9) = 1;
      *(char *)(param_1 + 0x7c) = param_2;
      if ((param_2 != '\0') && (0 < param_3)) {
        *(int *)(param_1 + 0x78) = param_3;
      }
      if (param_1 + 0xa8 != param_4) {
        FUN_180006470(param_1 + 0xa8,param_4,0,0xffffffffffffffff);
      }
      *(undefined1 *)(param_1 + 0x128) = 0;
      *(undefined4 *)(param_1 + 300) = 0;
      *(undefined8 *)(param_1 + 0x1a0) = param_5;
      *(undefined1 *)(param_1 + 0x1e0) = param_6;
      uStack_98 = FUN_1800e2720(param_1);
      if (uStack_98 == 0) {
        iVar6 = ResetEvent(*(undefined8 *)(param_1 + 0x80));
        if (iVar6 == 0) {
          GetLastError();
        }
        iVar6 = ResetEvent(*(undefined8 *)(param_1 + 0x88));
        if (iVar6 == 0) {
          GetLastError();
        }
        iVar6 = ResetEvent(*(undefined8 *)(param_1 + 0x90));
        if (iVar6 == 0) {
          GetLastError();
        }
        if (param_2 == '\0') {
          uVar11 = *(undefined4 *)(param_1 + 0x50);
          uVar8 = FUN_18008f040();
          FUN_1800ef740(uVar8,uVar11);
          FUN_1800ef800(uVar8,uVar11);
          FUN_1800e0030(param_1);
        }
        cVar5 = FUN_1800e1ec0(*(undefined4 *)(param_1 + 0x50));
        if (cVar5 == '\0') {
          uStack_80 = FUN_1800dd330;
          uStack_78 = (longlong *)((ulonglong)uStack_78._4_4_ << 0x20);
          puStack_38 = (undefined8 *)0x0;
          puVar9 = (undefined8 *)__2_YAPEAX_K_Z(0x28);
          if (puVar9 == (undefined8 *)0x0) {
            __Xbad_alloc_std__YAXXZ();
            pcVar3 = (code *)swi(3);
            uVar10 = (*pcVar3)();
            return uVar10;
          }
          *puVar9 = &UNK_18022d650;
          *(undefined4 *)(puVar9 + 1) = (undefined4)uStack_80;
          *(undefined4 *)((longlong)puVar9 + 0xc) = uStack_80._4_4_;
          *(undefined4 *)(puVar9 + 2) = (undefined4)uStack_78;
          *(uint *)((longlong)puVar9 + 0x14) = uStack_78._4_4_;
          puVar9[3] = param_1;
          puStack_38 = puVar9;
          uVar8 = FUN_18008e610();
          FUN_18015aaf0(uVar8,auStack_50,1);
        }
        else {
          uStack_80 = FUN_1800dd330;
          uStack_78 = (longlong *)((ulonglong)uStack_78._4_4_ << 0x20);
          puStack_58 = (undefined8 *)0x0;
          puVar9 = (undefined8 *)__2_YAPEAX_K_Z(0x28);
          if (puVar9 == (undefined8 *)0x0) {
            __Xbad_alloc_std__YAXXZ();
            pcVar3 = (code *)swi(3);
            uVar10 = (*pcVar3)();
            return uVar10;
          }
          *puVar9 = &UNK_18022d650;
          *(undefined4 *)(puVar9 + 1) = (undefined4)uStack_80;
          *(undefined4 *)((longlong)puVar9 + 0xc) = uStack_80._4_4_;
          *(undefined4 *)(puVar9 + 2) = (undefined4)uStack_78;
          *(uint *)((longlong)puVar9 + 0x14) = uStack_78._4_4_;
          puVar9[3] = param_1;
          puStack_58 = puVar9;
          uVar8 = FUN_18008e610();
          FUN_18015aaf0(uVar8,auStack_70,2);
        }
      }
      else {
        *(undefined1 *)(param_1 + 0xe9) = 0;
      }
    }
    else {
      uStack_98 = 0xe0000311;
    }
  }
  else {
    uStack_98 = 0xe000031d;
  }
  uVar10 = (ulonglong)uStack_98;
LAB_1800dd007:
  iVar6 = _Mtx_unlock(param_1 + 0x100);
  if (iVar6 != 0) {
    __Throw_C_error_std__YAXH_Z(iVar6);
  }
  return uVar10;
}

