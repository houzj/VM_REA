
void FUN_1800d34a0(longlong param_1,int param_2,int param_3,ulonglong param_4)

{
  code *pcVar1;
  undefined8 *puVar2;
  char cVar3;
  int iVar4;
  longlong lVar5;
  longlong *plVar6;
  undefined8 *puVar7;
  longlong lVar8;
  undefined8 uVar9;
  int *piVar10;
  undefined8 *puVar11;
  undefined8 *puVar12;
  undefined8 *puStackX_8;
  int aiStack_60 [2];
  ulonglong uStack_58;
  int aiStack_50 [2];
  ulonglong uStack_48;
  undefined1 auStack_40 [24];
  
  FUN_1801e5430();
  puVar7 = *(undefined8 **)(param_1 + 0x20);
  cVar3 = *(char *)((longlong)puVar7[1] + 0x19);
  puVar12 = puVar7;
  puVar2 = (undefined8 *)puVar7[1];
  while (cVar3 == '\0') {
    if ((*(int *)(puVar2 + 4) < param_3) ||
       ((*(int *)(puVar2 + 4) <= param_3 && ((ulonglong)puVar2[5] < param_4)))) {
      puVar11 = (undefined8 *)puVar2[2];
    }
    else {
      puVar11 = (undefined8 *)*puVar2;
      puVar12 = puVar2;
    }
    puVar2 = puVar11;
    cVar3 = *(char *)((longlong)puVar11 + 0x19);
  }
  puStackX_8 = puVar7;
  if (((puVar12 != puVar7) && (*(int *)(puVar12 + 4) <= param_3)) &&
     ((*(int *)(puVar12 + 4) < param_3 || ((ulonglong)puVar12[5] <= param_4)))) {
    puStackX_8 = puVar12;
  }
  aiStack_60[0] = param_2;
  uStack_58 = param_4;
  aiStack_50[0] = param_3;
  uStack_48 = param_4;
  if (puStackX_8 == puVar7) {
    lVar5 = CreateEventW(0,1,0);
    if (lVar5 == 0) {
      cVar3 = FUN_18001dbc0(4);
      if (cVar3 != '\0') {
        uVar9 = FUN_18001db50();
        FUN_180015b40(uVar9,4,&UNK_18022bcd8,&UNK_18022bc38,0x51);
      }
    }
    else {
      plVar6 = (longlong *)FUN_1800d6310(param_1 + 0x20,aiStack_50);
      *plVar6 = lVar5;
    }
  }
  else {
    puVar7 = (undefined8 *)FUN_1800d6310(param_1 + 0x20,aiStack_50);
    iVar4 = ResetEvent(*puVar7);
    if (iVar4 == 0) {
      GetLastError();
    }
  }
  puVar7 = *(undefined8 **)(param_1 + 0x30);
  cVar3 = *(char *)((longlong)puVar7[1] + 0x19);
  puVar12 = puVar7;
  puVar2 = (undefined8 *)puVar7[1];
  while (cVar3 == '\0') {
    if ((*(int *)(puVar2 + 4) < aiStack_60[0]) ||
       ((*(int *)(puVar2 + 4) <= aiStack_60[0] && ((ulonglong)puVar2[5] < uStack_58)))) {
      puVar11 = (undefined8 *)puVar2[2];
    }
    else {
      puVar11 = (undefined8 *)*puVar2;
      puVar12 = puVar2;
    }
    puVar2 = puVar11;
    cVar3 = *(char *)((longlong)puVar11 + 0x19);
  }
  puStackX_8 = puVar7;
  if (((puVar12 != puVar7) && (*(int *)(puVar12 + 4) <= aiStack_60[0])) &&
     ((*(int *)(puVar12 + 4) < aiStack_60[0] || ((ulonglong)puVar12[5] <= uStack_58)))) {
    puStackX_8 = puVar12;
  }
  if (puStackX_8 == puVar7) {
    lVar5 = CreateEventW(0,1,0);
    if (lVar5 == 0) {
      cVar3 = FUN_18001dbc0(4);
      if (cVar3 != '\0') {
        uVar9 = FUN_18001db50();
        FUN_180015b40(uVar9,4,&UNK_18022bcd8,&UNK_18022bc38,100);
      }
    }
    else {
      plVar6 = (longlong *)FUN_1800d6310(param_1 + 0x30,aiStack_60);
      *plVar6 = lVar5;
    }
  }
  else {
    puVar7 = (undefined8 *)FUN_1800d6310(param_1 + 0x30,aiStack_60);
    iVar4 = ResetEvent(*puVar7);
    if (iVar4 == 0) {
      GetLastError();
    }
  }
  lVar5 = *(longlong *)(param_1 + 0x50);
  lVar8 = FUN_1800d70a0();
  if (*(longlong *)(param_1 + 0x58) == 0x7fffffffffffffe) {
    __Xlength_error_std__YAXPEBD_Z(&UNK_18021d318);
    pcVar1 = (code *)swi(3);
    (*pcVar1)();
    return;
  }
  *(longlong *)(param_1 + 0x58) = *(longlong *)(param_1 + 0x58) + 1;
  *(longlong *)(lVar5 + 8) = lVar8;
  **(longlong **)(lVar8 + 8) = lVar8;
  uVar9 = FUN_1800d6480(param_1 + 0x60,aiStack_60);
  FUN_1800d6ca0(uVar9,auStack_40);
  piVar10 = (int *)FUN_1800d6540(param_1 + 0x70,aiStack_50);
  *piVar10 = aiStack_60[0];
  *(ulonglong *)(piVar10 + 2) = uStack_58;
  iVar4 = SetEvent(*(undefined8 *)(param_1 + 0x10));
  if (iVar4 == 0) {
    GetLastError();
  }
  FUN_1801e5390(param_1);
  return;
}

