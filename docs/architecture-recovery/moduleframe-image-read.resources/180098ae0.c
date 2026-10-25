
void FUN_180098ae0(longlong param_1,ulonglong param_2,longlong ****param_3,ulonglong param_4,
                  longlong ****param_5,longlong ****param_6,undefined8 param_7,undefined8 param_8,
                  longlong ****param_9,char param_10)

{
  longlong *plVar1;
  int *piVar2;
  longlong lVar3;
  longlong *plVar4;
  char cVar5;
  int iVar6;
  longlong lVar7;
  undefined8 uVar8;
  longlong lVar9;
  longlong ****pppplVar10;
  longlong ****pppplVar11;
  undefined1 auStack_158 [32];
  longlong ***ppplStack_138;
  longlong ***ppplStack_130;
  longlong ***ppplStack_128;
  longlong ***ppplStack_120;
  uint auStack_118 [2];
  uint uStack_110;
  longlong lStack_108;
  longlong *plStack_100;
  longlong ***ppplStack_f8;
  undefined8 uStack_f0;
  undefined *puStack_e8;
  char acStack_d8 [128];
  ulonglong uStack_58;
  
  uStack_f0 = 0xfffffffffffffffe;
  uStack_58 = DAT_1802a5230 ^ (ulonglong)auStack_158;
  uStack_110 = (uint)param_4;
  auStack_118[0] = (uint)param_2;
  lVar9 = 0;
  ppplStack_f8 = (longlong ***)0x0;
  pppplVar10 = param_3;
  if ((longlong ***)0xf < param_3[3]) {
    pppplVar10 = (longlong ****)*param_3;
  }
  if ((DAT_1802b3358 & 1) == 0) {
    DAT_1802b3358 = DAT_1802b3358 | 1;
    FUN_18008f080();
    FUN_1801e8930(&DAT_180202f60);
    param_2 = (ulonglong)auStack_118[0];
    param_4 = (ulonglong)uStack_110;
  }
  if ((int)param_2 != -1) {
    ppplStack_120 = (longlong ***)&ppplStack_f8;
    ppplStack_128 = (longlong ***)param_9;
    ppplStack_130 = (longlong ***)param_6;
    ppplStack_138 = (longlong ***)param_5;
    iVar6 = FUN_1800eee30(&DAT_1802b3360,param_2,pppplVar10,param_4);
    pppplVar10 = (longlong ****)ppplStack_f8;
    if (iVar6 == 0) goto LAB_180098c07;
  }
  cVar5 = FUN_18001dbc0(4);
  pppplVar10 = param_5;
  if (cVar5 != '\0') {
    pppplVar11 = param_3;
    if ((longlong ***)0xf < param_3[3]) {
      pppplVar11 = (longlong ****)*param_3;
    }
    uVar8 = FUN_18001db50();
    ppplStack_120 = (longlong ***)param_6;
    ppplStack_130 = (longlong ***)CONCAT44(ppplStack_130._4_4_,auStack_118[0]);
    ppplStack_138 = (longlong ***)CONCAT44(ppplStack_138._4_4_,0x4a9);
    ppplStack_128 = (longlong ***)pppplVar11;
    FUN_180015b40(uVar8,4,&DAT_180226a00,&DAT_180226a58);
  }
LAB_180098c07:
  if (param_10 != '\0') {
    lVar7 = FUN_18009a990(param_1 + 0x18,auStack_118);
    lVar7 = FUN_180007510(*(undefined8 *)(lVar7 + 0x18));
    FUN_18003fc70(lVar7,&DAT_180225850);
    *(uint *)(lVar7 + 0x10) = *(uint *)(lVar7 + 0x10) | 1;
    pppplVar11 = *(longlong *****)(lVar7 + 0x18);
    if (pppplVar11 == DAT_1802b05e8) {
      FUN_180001ac0(lVar7 + 0x18,param_3);
    }
    else if (pppplVar11 != param_3) {
      FUN_180006470(pppplVar11,param_3,0,0xffffffffffffffff);
    }
    acStack_d8[0] = '\0';
    memset(acStack_d8 + 1,0,0x7f);
    ppplStack_130 = (longlong ***)param_9;
    ppplStack_138 = (longlong ***)pppplVar10;
    FUN_18009ae50(acStack_d8,0x80,&DAT_180226ab8,param_6);
    uVar8 = FUN_18000de60(lVar7 + 0x28);
    if (acStack_d8[0] != '\0') {
      lVar7 = -1;
      do {
        lVar9 = lVar7 + 1;
        lVar3 = lVar7 + 1;
        lVar7 = lVar9;
      } while (acStack_d8[lVar3] != '\0');
    }
    FUN_180006300(uVar8,acStack_d8,lVar9);
  }
  puStack_e8 = &DAT_1802b1208;
  FUN_1801e5430(&DAT_1802b1208);
  if ((DAT_1802b2b48 & 1) == 0) {
    DAT_1802b2b48 = DAT_1802b2b48 | 1;
    FUN_1800c8f10(&DAT_1802b2b50);
    FUN_1801e8930(&DAT_180202a80);
  }
  if (pppplVar10 != (longlong ****)0x0) {
    FUN_1800cae00(&DAT_1802b2b50,&lStack_108,auStack_118[0]);
    plVar4 = plStack_100;
    if (lStack_108 == 0) {
      if (plStack_100 != (longlong *)0x0) {
        LOCK();
        plVar1 = plStack_100 + 1;
        *(int *)plVar1 = (int)*plVar1 + -1;
        UNLOCK();
        if ((int)*plVar1 == 0) {
          (**(code **)*plStack_100)(plStack_100);
          LOCK();
          piVar2 = (int *)((longlong)plVar4 + 0xc);
          *piVar2 = *piVar2 + -1;
          UNLOCK();
          if (*piVar2 == 0) {
            (**(code **)(*plStack_100 + 8))();
          }
        }
      }
    }
    else {
      ppplStack_120 = (longlong ***)param_9;
      ppplStack_138 = (longlong ***)param_6;
      FUN_1800f49a0(lStack_108,param_3,uStack_110,pppplVar10);
      plVar4 = plStack_100;
      if (plStack_100 != (longlong *)0x0) {
        LOCK();
        plVar1 = plStack_100 + 1;
        *(int *)plVar1 = (int)*plVar1 + -1;
        UNLOCK();
        if ((int)*plVar1 == 0) {
          (**(code **)*plStack_100)(plStack_100);
          LOCK();
          piVar2 = (int *)((longlong)plVar4 + 0xc);
          *piVar2 = *piVar2 + -1;
          UNLOCK();
          if (*piVar2 == 0) {
            (**(code **)(*plStack_100 + 8))();
          }
        }
      }
    }
  }
  FUN_1801e5390(&DAT_1802b1208);
  FUN_1801e8810(uStack_58 ^ (ulonglong)auStack_158);
  return;
}

