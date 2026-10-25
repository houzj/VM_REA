
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void FUN_180013f10(undefined8 param_1,undefined8 param_2,undefined4 param_3,byte param_4,
                  int *param_5)

{
  byte *pbVar1;
  char cVar2;
  int iVar3;
  char *pcVar4;
  byte bVar5;
  undefined8 uVar6;
  bool bVar7;
  undefined1 auStack_98 [32];
  int *piStack_78;
  undefined1 auStack_68 [32];
  ulonglong uStack_48;
  
  uStack_48 = _DAT_180025000 ^ (ulonglong)auStack_98;
  bVar5 = 0;
  FUN_180018930(auStack_68,&UNK_18001d008);
  iVar3 = FUN_180014140(param_1,param_2,auStack_68,param_5);
  if (iVar3 == 0) {
    iVar3 = *param_5;
    bVar7 = false;
    if (iVar3 == 1) {
      iVar3 = param_5[2];
LAB_18001401c:
      bVar7 = iVar3 == 0;
    }
    else if (iVar3 == 2) {
      if ((float)param_5[2] == 0.0) {
        bVar7 = true;
      }
      else {
        bVar7 = false;
      }
    }
    else if (iVar3 == 3) {
      if (param_5[1] == 3) {
        iVar3 = 0;
        pcVar4 = *(char **)(param_5 + 2);
        do {
          cVar2 = toupper((int)*pcVar4);
          iVar3 = iVar3 + 1;
          *pcVar4 = cVar2;
          pcVar4 = pcVar4 + 1;
        } while (iVar3 < param_5[1] + -1);
      }
      pbVar1 = *(byte **)(param_5 + 2);
      iVar3 = (uint)*pbVar1 - (uint)DAT_18001cf84;
      if ((iVar3 == 0) && (iVar3 = (uint)pbVar1[1] - (uint)DAT_18001cf85, iVar3 == 0)) {
        iVar3 = (uint)pbVar1[2] - (uint)DAT_18001cf86;
      }
      goto LAB_18001401c;
    }
    uVar6 = 2;
    if (bVar7) {
      uVar6 = 4;
    }
    piStack_78 = param_5;
    iVar3 = FUN_1800140a0(param_1,param_2,param_3,uVar6);
    if (iVar3 == 0) goto LAB_18001404a;
  }
  bVar5 = 1;
LAB_18001404a:
  if ((param_4 & bVar5) != 0) {
    FUN_180018930(auStack_68,&UNK_18001d040,param_3);
    FUN_180014140(param_1,param_2,auStack_68,param_5);
  }
  FUN_180018cd0(uStack_48 ^ (ulonglong)auStack_98);
  return;
}

