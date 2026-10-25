module.exports=function(edges){
 const {rva,refs}=require('./verify-resource-iv.cjs')();
 edges.push({id:'E67',route:'原版 · 资源保护静态参数',classification:'source-confirmed',from:'AesCryptHelper..cctor / FieldDef 0x040001a1',to:'16字节RVA数据 → MY_IV → Encrypt/Decrypt.set_IV',behavior:'原静态构造分配16字节数组，用InitializeArray读取私有实现数据字段后写MY_IV。已从同一原件按FieldRVA、值类型ClassLayout.Size=16、PE节/文件偏移边界读取16字节，保留原字节与摘要并核REA字段签名。加密/解密两体均ldsfld MY_IV后set_IV；这是随包固定初始化数据，不是每次调用现场生成随机IV的证据。',refs,unknowns:['数据初始化已定位；数组的所有潜在间接修改、运行程序集/提供器选择、模式/填充、真实密文差分及原生.sol算法仍未证明。没有加载或执行厂商程序集，也未解密用户资源。']});
 edges.find(e=>e.id==='E35').unknowns=['IV原始16字节RVA数据及到set_IV的链见E67；运行提供器、最终模式/填充合同和无效长度行为仍待核。两流有finally释放，但算法对象在这些体中无显式Dispose。'];
 return {fieldToken:rva.fieldToken,bytes:rva.byteLength,sha256:rva.dataSha256,runtimeExecuted:false};
};
