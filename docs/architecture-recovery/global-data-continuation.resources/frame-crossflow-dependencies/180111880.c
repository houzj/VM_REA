
/* WARNING: Removing unreachable block (ram,0x000180111a1f) */
/* WARNING: Removing unreachable block (ram,0x000180111a24) */
/* WARNING: Removing unreachable block (ram,0x000180111a2d) */
/* WARNING: Removing unreachable block (ram,0x000180111a51) */
/* WARNING: Removing unreachable block (ram,0x000180111a5b) */
/* WARNING: Removing unreachable block (ram,0x000180111a60) */
/* WARNING: Removing unreachable block (ram,0x000180111a66) */
/* WARNING: Removing unreachable block (ram,0x000180111a73) */
/* WARNING: Removing unreachable block (ram,0x000180111a37) */
/* WARNING: Removing unreachable block (ram,0x000180111a43) */
/* WARNING: Removing unreachable block (ram,0x000180111a4f) */
/* WARNING: Removing unreachable block (ram,0x000180111a76) */
/* WARNING: Removing unreachable block (ram,0x000180111bbf) */
/* WARNING: Removing unreachable block (ram,0x000180111bc4) */
/* WARNING: Removing unreachable block (ram,0x000180111bcd) */
/* WARNING: Removing unreachable block (ram,0x000180111bf1) */
/* WARNING: Removing unreachable block (ram,0x000180111bfb) */
/* WARNING: Removing unreachable block (ram,0x000180111c00) */
/* WARNING: Removing unreachable block (ram,0x000180111c06) */
/* WARNING: Removing unreachable block (ram,0x000180111c13) */
/* WARNING: Removing unreachable block (ram,0x000180111bd7) */
/* WARNING: Removing unreachable block (ram,0x000180111be3) */
/* WARNING: Removing unreachable block (ram,0x000180111bef) */
/* WARNING: Removing unreachable block (ram,0x000180111c16) */

int FUN_180111880(longlong param_1,int param_2,undefined8 param_3)

{
  char cVar1;
  undefined8 *puVar2;
  longlong *plVar3;
  longlong *plVar4;
  int iVar5;
  longlong *plVar6;
  undefined8 *puVar7;
  undefined8 *puVar8;
  undefined8 *puStackX_8;
  longlong *plStack_50;
  undefined8 uStack_48;
  undefined8 *puStack_40;
  undefined8 uStack_38;
  
  puVar2 = *(undefined8 **)(param_1 + 0xb0);
  cVar1 = *(char *)((longlong)puVar2[1] + 0x19);
  puStackX_8 = puVar2;
  puVar8 = (undefined8 *)puVar2[1];
  while (cVar1 == '\0') {
    if (*(int *)(puVar8 + 4) < param_2) {
      puVar7 = (undefined8 *)puVar8[2];
      puVar8 = puStackX_8;
    }
    else {
      puVar7 = (undefined8 *)*puVar8;
    }
    puStackX_8 = puVar8;
    puVar8 = puVar7;
    cVar1 = *(char *)((longlong)puVar7 + 0x19);
  }
  if ((puStackX_8 == puVar2) || (param_2 < *(int *)(puStackX_8 + 4))) {
    puStackX_8 = puVar2;
  }
  if (puStackX_8 == *(undefined8 **)(param_1 + 0xb0)) {
    iVar5 = -0x1ffffce6;
  }
  else {
    plStack_50 = (longlong *)0x0;
    uStack_48 = 0;
    plStack_50 = (longlong *)FUN_180079000(&plStack_50);
    iVar5 = FUN_180112540(param_1,param_2,&plStack_50);
    if (iVar5 == 0) {
      puStack_40 = (undefined8 *)0x0;
      uStack_38 = 0;
      puStack_40 = (undefined8 *)FUN_180079000(&puStack_40);
      iVar5 = FUN_1801127a0(param_1,param_2,&puStack_40);
      if (iVar5 == 0) {
        plVar6 = (longlong *)*plStack_50;
        if (plVar6 != plStack_50) {
          do {
            iVar5 = FUN_1801122a0(param_1,*(undefined4 *)((longlong)plVar6 + 0x1c),&puStack_40,
                                  param_3);
            if (iVar5 != 0) break;
            if (*(char *)((longlong)plVar6 + 0x19) == '\0') {
              plVar3 = (longlong *)plVar6[2];
              if (*(char *)((longlong)plVar3 + 0x19) == '\0') {
                cVar1 = *(char *)(*plVar3 + 0x19);
                plVar6 = plVar3;
                plVar3 = (longlong *)*plVar3;
                while (cVar1 == '\0') {
                  cVar1 = *(char *)(*plVar3 + 0x19);
                  plVar6 = plVar3;
                  plVar3 = (longlong *)*plVar3;
                }
              }
              else {
                cVar1 = *(char *)(plVar6[1] + 0x19);
                plVar4 = (longlong *)plVar6[1];
                plVar3 = plVar6;
                while ((plVar6 = plVar4, cVar1 == '\0' && (plVar3 == (longlong *)plVar6[2]))) {
                  cVar1 = *(char *)(plVar6[1] + 0x19);
                  plVar4 = (longlong *)plVar6[1];
                  plVar3 = plVar6;
                }
              }
            }
          } while (plVar6 != plStack_50);
        }
        FUN_180071720(param_3,*puStack_40,puStack_40);
        cVar1 = *(char *)((longlong)puStack_40[1] + 0x19);
        plVar6 = (longlong *)puStack_40[1];
        while (cVar1 == '\0') {
          FUN_18003c6d0(&puStack_40,plVar6[2]);
          plVar3 = (longlong *)*plVar6;
          __3_YAXPEAX_Z(plVar6);
          plVar6 = plVar3;
          cVar1 = *(char *)((longlong)plVar3 + 0x19);
        }
        puStack_40[1] = puStack_40;
        *puStack_40 = puStack_40;
        puStack_40[2] = puStack_40;
        uStack_38 = 0;
        __3_YAXPEAX_Z(puStack_40);
        cVar1 = *(char *)(plStack_50[1] + 0x19);
        plVar6 = (longlong *)plStack_50[1];
        while (cVar1 == '\0') {
          FUN_18003c6d0(&plStack_50,plVar6[2]);
          plVar3 = (longlong *)*plVar6;
          __3_YAXPEAX_Z(plVar6);
          plVar6 = plVar3;
          cVar1 = *(char *)((longlong)plVar3 + 0x19);
        }
      }
      else {
        cVar1 = *(char *)((longlong)puStack_40[1] + 0x19);
        plVar6 = (longlong *)puStack_40[1];
        while (cVar1 == '\0') {
          FUN_18003c6d0(&puStack_40,plVar6[2]);
          plVar3 = (longlong *)*plVar6;
          __3_YAXPEAX_Z(plVar6);
          plVar6 = plVar3;
          cVar1 = *(char *)((longlong)plVar3 + 0x19);
        }
        puStack_40[1] = puStack_40;
        *puStack_40 = puStack_40;
        puStack_40[2] = puStack_40;
        uStack_38 = 0;
        __3_YAXPEAX_Z(puStack_40);
        cVar1 = *(char *)(plStack_50[1] + 0x19);
        plVar6 = (longlong *)plStack_50[1];
        while (cVar1 == '\0') {
          FUN_18003c6d0(&plStack_50,plVar6[2]);
          plVar3 = (longlong *)*plVar6;
          __3_YAXPEAX_Z(plVar6);
          plVar6 = plVar3;
          cVar1 = *(char *)((longlong)plVar3 + 0x19);
        }
      }
      plStack_50[1] = (longlong)plStack_50;
      *plStack_50 = (longlong)plStack_50;
      plStack_50[2] = (longlong)plStack_50;
      uStack_48 = 0;
      __3_YAXPEAX_Z(plStack_50);
    }
    else {
      cVar1 = *(char *)(plStack_50[1] + 0x19);
      plVar6 = (longlong *)plStack_50[1];
      while (cVar1 == '\0') {
        FUN_18003c6d0(&plStack_50,plVar6[2]);
        plVar3 = (longlong *)*plVar6;
        __3_YAXPEAX_Z(plVar6);
        plVar6 = plVar3;
        cVar1 = *(char *)((longlong)plVar3 + 0x19);
      }
      plStack_50[1] = (longlong)plStack_50;
      *plStack_50 = (longlong)plStack_50;
      plStack_50[2] = (longlong)plStack_50;
      uStack_48 = 0;
      __3_YAXPEAX_Z(plStack_50);
    }
  }
  return iVar5;
}

