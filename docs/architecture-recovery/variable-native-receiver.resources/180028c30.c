
int FUN_180028c30(longlong param_1,char *param_2,byte *param_3,float *param_4,int param_5)

{
  byte bVar1;
  undefined1 uVar2;
  uint uVar3;
  byte *pbVar4;
  longlong lVar5;
  int local_14;
  
  if (((param_2 == (char *)0x0) || (param_2 == (char *)0x0)) || (param_4 == (float *)0x0)) {
    local_14 = -0x1fffffff;
  }
  else {
    local_14 = 0;
    uVar2 = VM_M_SetGlobalVarLockStatus
                      (*(undefined4 *)(param_1 + 0x48),*(undefined4 *)(param_1 + 0x48),1);
    pbVar4 = param_3;
    do {
      bVar1 = *pbVar4;
      if (bVar1 != pbVar4[(longlong)&DAT_180057a8c - (longlong)param_3]) {
        uVar3 = -(uint)(bVar1 < pbVar4[(longlong)&DAT_180057a8c - (longlong)param_3]) | 1;
        goto LAB_180028cb6;
      }
      pbVar4 = pbVar4 + 1;
    } while (bVar1 != 0);
    uVar3 = 0;
LAB_180028cb6:
    if (uVar3 == 0) {
      local_14 = FUN_1800295b0(param_1,param_2,(longlong)param_4,param_5,uVar2,'\0');
    }
    else {
      pbVar4 = param_3;
      do {
        bVar1 = *pbVar4;
        if (bVar1 != pbVar4[(longlong)"float" - (longlong)param_3]) {
          uVar3 = -(uint)(bVar1 < pbVar4[(longlong)"float" - (longlong)param_3]) | 1;
          goto LAB_180028d11;
        }
        pbVar4 = pbVar4 + 1;
      } while (bVar1 != 0);
      uVar3 = 0;
LAB_180028d11:
      if (uVar3 == 0) {
        local_14 = FUN_180029eb0(param_1,param_2,param_4,param_5,uVar2,'\0');
      }
      else {
        pbVar4 = param_3;
        do {
          bVar1 = *pbVar4;
          if (bVar1 != pbVar4[(longlong)"string" - (longlong)param_3]) {
            uVar3 = -(uint)(bVar1 < pbVar4[(longlong)"string" - (longlong)param_3]) | 1;
            goto LAB_180028d6c;
          }
          pbVar4 = pbVar4 + 1;
        } while (bVar1 != 0);
        uVar3 = 0;
LAB_180028d6c:
        if (uVar3 == 0) {
          local_14 = FUN_18002a7b0(param_1,param_2,(longlong)param_4,param_5,uVar2,'\0');
        }
        else {
          pbVar4 = param_3;
          do {
            bVar1 = *pbVar4;
            if (bVar1 != pbVar4[(longlong)&DAT_1800588a0 - (longlong)param_3]) {
              uVar3 = -(uint)(bVar1 < pbVar4[(longlong)&DAT_1800588a0 - (longlong)param_3]) | 1;
              goto LAB_180028dc7;
            }
            pbVar4 = pbVar4 + 1;
          } while (bVar1 != 0);
          uVar3 = 0;
LAB_180028dc7:
          if (uVar3 == 0) {
            local_14 = FUN_18002b270(param_1,(longlong)param_2,(longlong)param_4,param_5,uVar2);
          }
          else {
            pbVar4 = param_3;
            do {
              bVar1 = *pbVar4;
              if (bVar1 != pbVar4[(longlong)"image" - (longlong)param_3]) {
                uVar3 = -(uint)(bVar1 < pbVar4[(longlong)"image" - (longlong)param_3]) | 1;
                goto LAB_180028e22;
              }
              pbVar4 = pbVar4 + 1;
            } while (bVar1 != 0);
            uVar3 = 0;
LAB_180028e22:
            if (uVar3 == 0) {
              local_14 = FUN_18002b2e0(param_1,param_2,param_4,param_5,0);
            }
            else {
              lVar5 = (longlong)"pointset" - (longlong)param_3;
              do {
                bVar1 = *param_3;
                if (bVar1 != param_3[lVar5]) {
                  uVar3 = -(uint)(bVar1 < param_3[lVar5]) | 1;
                  goto LAB_180028e7d;
                }
                param_3 = param_3 + 1;
              } while (bVar1 != 0);
              uVar3 = 0;
LAB_180028e7d:
              if (uVar3 == 0) {
                local_14 = FUN_18002b530(param_1,(longlong)param_2,(longlong)param_4,param_5,0,uVar2
                                        );
              }
            }
          }
        }
      }
    }
    VM_M_SetGlobalVarLockStatus(*(undefined4 *)(param_1 + 0x48),*(undefined4 *)(param_1 + 0x48),0);
  }
  return local_14;
}

