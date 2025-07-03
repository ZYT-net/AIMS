<template>
  <div>
    <h2>作物管理</h2>

    <!-- 新增作物表单 -->
    <div class="add-form" style="margin-bottom: 20px; padding: 15px; border: 1px solid #ebeef5; border-radius: 4px; background-color: #fafafa;">
      <h3>添加作物</h3>
      <el-form :model="newCrop" label-width="80px" inline>
        <el-form-item label="作物名称">
          <el-input v-model="newCrop.name" placeholder="请输入作物名称" style="width: 200px;"></el-input>
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="addCrop" :disabled="!newCrop.name">添加</el-button>
        </el-form-item>
      </el-form>
    </div>

    <!-- 作物列表 -->
    <el-table :data="crops" style="width: 100%; margin-bottom: 20px;">
      <el-table-column prop="name" label="作物名称" />
      <el-table-column label="操作">
        <template #default="scope">
          <el-button size="small" type="warning" @click="editCrop(scope.row)">编辑</el-button>
          <el-button size="small" type="danger" @click="deleteCrop(scope.row.id)">删除</el-button>
        </template>
      </el-table-column>
    </el-table>

    <!-- 编辑作物表单 (默认隐藏) -->
    <div v-if="editingCrop.id" class="edit-form" style="margin-bottom: 20px; padding: 15px; border: 1px solid #ebeef5; border-radius: 4px; background-color: #fafafa;">
      <h3>编辑作物</h3>
      <el-form :model="editingCrop" label-width="80px" inline>
        <el-form-item label="作物名称">
          <el-input v-model="editingCrop.name" placeholder="请输入作物名称" style="width: 200px;"></el-input>
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="saveEditedCrop">保存</el-button>
          <el-button @click="cancelEdit">取消</el-button>
        </el-form-item>
      </el-form>
    </div>
  </div>
</template>

<script setup>
  import { ref, onMounted } from 'vue'
  import api from '@/api'

  // 作物列表
  const crops = ref([])
  // 新增作物表单数据
  const newCrop = ref({ name: '' })
  // 正在编辑的作物
  const editingCrop = ref({ id: null, name: '' })

  // 获取作物列表
  const fetchCrops = async () => {
    try {
      const res = await api.get('/crops')
      crops.value = res.data
    } catch (e) {
      console.error('获取作物列表失败', e)
      crops.value = []
    }
  }

  onMounted(fetchCrops)

  // 添加作物
  const addCrop = async () => {
    try {
      await api.post('/crops', newCrop.value)
      newCrop.value.name = '' // 清空输入框
      await fetchCrops()
    } catch (e) {
      console.error('添加作物失败', e)
    }
  }

  // 编辑作物
  const editCrop = (crop) => {
    editingCrop.value = { ...crop }
  }

  // 保存编辑的作物
  const saveEditedCrop = async () => {
    try {
      await api.put(`/crops/${editingCrop.value.id}`, editingCrop.value)
      editingCrop.value = { id: null, name: '' } // 重置编辑状态
      await fetchCrops()
    } catch (e) {
      console.error('保存编辑失败', e)
    }
  }

  // 取消编辑
  const cancelEdit = () => {
    editingCrop.value = { id: null, name: '' }
  }

  // 删除作物
  const deleteCrop = async (id) => {
    if (confirm('确定要删除此作物吗？')) {
      try {
        await api.delete(`/crops/${id}`)
        await fetchCrops()
      } catch (e) {
        console.error('删除作物失败', e)
      }
    }
  }
</script>

