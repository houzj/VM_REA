
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void FUN_1800997c0(undefined4 param_1,undefined4 param_2,undefined8 *param_3,undefined8 *param_4,
                  int param_5,char param_6)

{
  longlong *plVar1;
  int *piVar2;
  char *pcVar3;
  longlong *plVar4;
  char cVar5;
  int iVar6;
  int iVar7;
  longlong lVar8;
  undefined8 uVar9;
  longlong in_RCX;
  undefined8 *puVar10;
  longlong lVar11;
  longlong lVar12;
  undefined1 auStack_108 [32];
  undefined4 uStack_e8;
  undefined4 uStack_e0;
  undefined8 *puStack_d8;
  undefined8 uStack_d0;
  undefined4 auStack_c8 [2];
  longlong lStack_c0;
  longlong *plStack_b8;
  longlong lStack_b0;
  longlong *plStack_a8;
  undefined1 auStack_a0 [16];
  longlong lStack_90;
  undefined8 uStack_88;
  undefined *puStack_80;
  undefined8 uStack_78;
  undefined1 uStack_70;
  undefined7 uStack_6f;
  longlong lStack_60;
  ulonglong uStack_58;
  ulonglong uStack_50;
  
  uStack_78 = 0xfffffffffffffffe;
  uStack_50 = DAT_1802a5230 ^ (ulonglong)auStack_108;
  lVar12 = (longlong)param_5;
  auStack_c8[0] = param_2;
  if (param_5 != 0) {
    if (param_6 != '\0') {
      lVar8 = FUN_18009a990(in_RCX + 0x18,auStack_c8);
      lVar8 = FUN_180007510(*(undefined8 *)(lVar8 + 0x18));
      param_1 = FUN_18003fc70(lVar8,&DAT_1802260f8);
      *(uint *)(lVar8 + 0x10) = *(uint *)(lVar8 + 0x10) | 1;
      puVar10 = *(undefined8 **)(lVar8 + 0x18);
      if (puVar10 == DAT_1802b05e8) {
        param_1 = FUN_180001ac0(lVar8 + 0x18,param_3);
      }
      else if (puVar10 != param_3) {
        param_1 = FUN_180006470(puVar10,param_3,0,0xffffffffffffffff);
      }
      if (0 < param_5) {
        puVar10 = param_4;
        do {
          pcVar3 = (char *)*puVar10;
          uVar9 = FUN_18000de60(lVar8 + 0x28);
          if (*pcVar3 != '\0') {
            lVar11 = -1;
            do {
              lVar11 = lVar11 + 1;
            } while (pcVar3[lVar11] != '\0');
          }
          param_1 = FUN_180006300(uVar9,pcVar3);
          puVar10 = puVar10 + 2;
          lVar12 = lVar12 + -1;
        } while (lVar12 != 0);
      }
    }
    iVar6 = FUN_18009a7e0(param_1,auStack_c8[0],param_3,3);
    if ((iVar6 != 0) && (cVar5 = FUN_18001dbc0(4), cVar5 != '\0')) {
      if (0xf < (ulonglong)param_3[3]) {
        param_3 = (undefined8 *)*param_3;
      }
      uVar9 = FUN_18001db50();
      uStack_d0 = CONCAT44(uStack_d0._4_4_,iVar6);
      uStack_e0 = auStack_c8[0];
      uStack_e8 = 0x544;
      puStack_d8 = param_3;
      FUN_180015b40(uVar9,4,&DAT_180226930,&DAT_180226b20);
    }
    puStack_80 = &DAT_1802b1208;
    FUN_1801e5430(&DAT_1802b1208);
    iVar6 = 0;
    puVar10 = param_4;
    if (0 < param_5) {
      do {
        lVar8 = 0;
        pcVar3 = (char *)*puVar10;
        uStack_58 = 0xf;
        uStack_70 = 0;
        lVar12 = lVar8;
        if (*pcVar3 != '\0') {
          lVar12 = -1;
          do {
            lVar12 = lVar12 + 1;
          } while (pcVar3[lVar12] != '\0');
        }
        lStack_60 = lVar8;
        FUN_180006300(&uStack_70,pcVar3,lVar12);
        if ((DAT_1802b2b48 & 1) == 0) {
          DAT_1802b2b48 = DAT_1802b2b48 | 1;
          FUN_1800c8f10(&DAT_1802b2b50);
          FUN_1801e8930(&DAT_180202a80);
        }
        FUN_1800cae00(&DAT_1802b2b50,&lStack_c0);
        plVar4 = plStack_b8;
        if (lStack_c0 == 0) {
          if (plStack_b8 != (longlong *)0x0) {
            LOCK();
            plVar1 = plStack_b8 + 1;
            *(int *)plVar1 = (int)*plVar1 + -1;
            UNLOCK();
            if ((int)*plVar1 == 0) {
              (**(code **)*plStack_b8)(plStack_b8);
              LOCK();
              piVar2 = (int *)((longlong)plVar4 + 0xc);
              *piVar2 = *piVar2 + -1;
              UNLOCK();
              if (*piVar2 == 0) {
                (**(code **)(*plStack_b8 + 8))();
              }
            }
          }
          iVar7 = -0x1ffffffd;
        }
        else {
          FUN_1800f7260(lStack_c0,&lStack_b0);
          plVar4 = plStack_a8;
          if (lStack_b0 == 0) {
            if (plStack_a8 != (longlong *)0x0) {
              LOCK();
              plVar1 = plStack_a8 + 1;
              *(int *)plVar1 = (int)*plVar1 + -1;
              UNLOCK();
              if ((int)*plVar1 == 0) {
                (**(code **)*plStack_a8)(plStack_a8);
                LOCK();
                piVar2 = (int *)((longlong)plVar4 + 0xc);
                *piVar2 = *piVar2 + -1;
                UNLOCK();
                if (*piVar2 == 0) {
                  (**(code **)(*plVar4 + 8))(plVar4);
                }
              }
            }
            iVar7 = -0x1ffffffd;
          }
          else {
            uStack_88 = 0xf;
            auStack_a0[0] = 0;
            lStack_90 = lVar8;
            FUN_180006470(auStack_a0,&uStack_70,0,0xffffffffffffffff);
            iVar7 = FUN_18000ffe0(lStack_b0,iVar6);
            plVar4 = plStack_a8;
            if (plStack_a8 != (longlong *)0x0) {
              LOCK();
              plVar1 = plStack_a8 + 1;
              *(int *)plVar1 = (int)*plVar1 + -1;
              UNLOCK();
              if ((int)*plVar1 == 0) {
                (**(code **)*plStack_a8)(plStack_a8);
                LOCK();
                piVar2 = (int *)((longlong)plVar4 + 0xc);
                *piVar2 = *piVar2 + -1;
                UNLOCK();
                if (*piVar2 == 0) {
                  (**(code **)(*plVar4 + 8))(plVar4);
                }
              }
            }
          }
          plVar4 = plStack_b8;
          if (plStack_b8 != (longlong *)0x0) {
            LOCK();
            plVar1 = plStack_b8 + 1;
            *(int *)plVar1 = (int)*plVar1 + -1;
            UNLOCK();
            if ((int)*plVar1 == 0) {
              (**(code **)*plStack_b8)(plStack_b8);
              LOCK();
              piVar2 = (int *)((longlong)plVar4 + 0xc);
              *piVar2 = *piVar2 + -1;
              UNLOCK();
              if (*piVar2 == 0) {
                (**(code **)(*plStack_b8 + 8))();
              }
            }
          }
        }
        if (0xf < uStack_58) {
          __3_YAXPEAX_Z(CONCAT71(uStack_6f,uStack_70));
        }
        if (iVar7 != 0) {
          cVar5 = FUN_18001dbc0(4);
          if (cVar5 != '\0') {
            if ((DAT_1802b1218 & 1) == 0) {
              DAT_1802b1218 = DAT_1802b1218 | 1;
              _DAT_1802b1230 = 1;
              DAT_1802b1238 = 0;
              DAT_1802b1240 = 0;
              uRam00000001802b1248 = 0;
              DAT_1802b1250 = 0;
              uRam00000001802b1258 = 0;
              DAT_1802b1260 = 0;
              uRam00000001802b1268 = 0;
              _DAT_1802b1270 = 0;
              FUN_1801e8930(&DAT_180202850);
            }
            uStack_d0 = param_4[(longlong)iVar6 * 2];
            puStack_d8 = (undefined8 *)CONCAT44(puStack_d8._4_4_,iVar6);
            uStack_e0 = auStack_c8[0];
            uStack_e8 = 0x54d;
            FUN_180015b40(&DAT_1802b1230,4,&DAT_180226bc0,&DAT_180226b20);
          }
          break;
        }
        iVar6 = iVar6 + 1;
        puVar10 = puVar10 + 2;
      } while (iVar6 < param_5);
    }
    FUN_1801e5390(&DAT_1802b1208);
  }
  FUN_1801e8810(uStack_50 ^ (ulonglong)auStack_108);
  return;
}

