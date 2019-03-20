<script type="text/javascript" src="http://cdn.mathjax.org/mathjax/latest/MathJax.js?config=default"></script>
# 时空状态网的构建  
----
主要针对铁路乘务计划问题中的“绝对时间”约束（用餐时间窗）和“相对时间”约束（相关乘务规则）
两种网络构建方式：时空接续网（connection network）和时空轴线网（time-space network）  
前者易于描述“相对时间”约束，但网络规模大（点之间的连接弧的数量多，稠密图）；后者易于处理“绝对时间”约束（稀疏图），但模型中的复杂约束的处理是难点
***
为了结合两种网络的优点，提出**时空状态网(Time-Space-State Network)**（见周学松2016）。
该网络在时空轴线网络的基础上，增加一个维度——“状态”，构成一个三维的网络：时间、空间、状态。
***
**状态**：每个点的状态是指：从源点到当前点满足路径可行约束的可行状态。因此，多数约束都通过时空状态网中点的可行状态来描述，剩下的待处理的约束
只有交路的耦合约束和流平衡约束。  
基于TSSN的车辆路径优化模型的主要思想是将复杂的路径约束从优化模型中转移到网络结构中，用经典的优化方法求解车辆路径问题。而在TSSN中的路径生成问题被转化为
经典的最短路问题，具有多项式时间解法。  
***
在CSP中，通过**状态**来描述根据“混合时间”约束推算得到的点（task）的可行状态，TSSN的结构适用于拉格朗日松弛来求解
## 问题描述与网络构建  
- 基本的乘务术语就不说了
- 乘务规则：  

1. working time of a duty \Td <= \Td^max:累计工作时间，包括驾驶、换乘、间休。 
2. transfer time \Tt >= \Tt_min:换乘时间。 
3. consecutive driving time \To <= \To^max:连续驾驶时间（包括换乘，不包括间休）
4. break time \Tr >= \Tr_min:间休时长
5. overnight rest time \Ts >= \Ts_min:外驻时间
6. the period of a pairing \Tp <= \Dd:交路长度（出乘-退乘 的总时长） 
7. meal break time:午餐 \TW{^l_MB} = [\ML_min, \ML_max] and 晚餐 \TW{^s_MB} = [\MS_min, MS_max\] 此外，用餐时间必须是工作开始后\Te^a_b小时和工作结束前\Te^b_f 小时

前6个：相对时间；第7个：绝对时间
  

参考文献：A Lagrangian Relaxation Approach 
