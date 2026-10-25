
void DestroyModule(undefined8 param_1,undefined8 *param_2)

{
                    /* 0xed40  23  DestroyModule */
  if (param_2 != (undefined8 *)0x0) {
                    /* WARNING: Could not recover jumptable at 0x00018000ed53. Too many branches */
                    /* WARNING: Treating indirect jump as call */
    (**(code **)*param_2)(param_2,1);
    return;
  }
  return;
}

