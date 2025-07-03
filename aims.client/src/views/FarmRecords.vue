<template>
  <div>
    <h2>农事记录</h2>

    <!-- 新增记录表单 -->
    <div class="add-form" style="margin-bottom: 20px; padding: 15px; border: 1px solid #ebeef5; border-radius: 4px; background-color: #fafafa;">
      <h3>添加农事记录</h3>
      <el-form :model="newRecord" label-width="80px" size="small">
        <el-form-item label="作物" required>
          <el-select v-model="newRecord.cropId" placeholder="请选择作物" style="width: 200px;">
            <el-option v-for="crop in crops"
                       :key="crop.id"
                       :label="`${crop.name}`"
                       :value="crop.id" />
          </el-select>
        </el-form-item>
        <el-form-item label="操作" required>
          <el-input v-model="newRecord.action" placeholder="请输入操作" style="width: 200px;"></el-input>
        </el-form-item>
        <el-form-item label="日期" required>
          <el-date-picker v-model="newRecord.recordDate"
                          type="date"
                          placeholder="选择日期"
                          style="width: 200px;">
          </el-date-picker>
        </el-form-item>
        <el-form-item label="描述">
          <el-input v-model="newRecord.description" placeholder="请输入描述" style="width: 200px;"></el-input>
        </el-form-item>
        <el-form-item>
          <el-button type="primary"
                     @click="addRecord"
                     :disabled="!newRecord.cropId || !newRecord.action || !newRecord.recordDate">
            添加
          </el-button>
        </el-form-item>
      </el-form>
    </div>

    <!-- 农事记录列表 -->
    <el-table :data="records" style="width: 100%; margin-bottom: 20px;">
      <el-table-column prop="cropName" label="作物" />
      <el-table-column prop="action" label="操作" />
      <el-table-column prop="recordDate" label="日期" />
      <el-table-column prop="description" label="描述" />
      <el-table-column label="操作">
        <template #default="scope">
          <el-button size="small" type="warning" @click="editRecord(scope.row)">编辑</el-button>
          <el-button size="small" type="danger" @click="deleteRecord(scope.row.id)">删除</el-button>
        </template>
      </el-table-column>
    </el-table>

    <!-- 编辑记录表单 (默认隐藏) -->
    <div v-if="editingRecord.id" class="edit-form" style="margin-bottom: 20px; padding: 15px; border: 1px solid #ebeef5; border-radius: 4px; background-color: #fafafa;">
      <h3>编辑农事记录</h3>
      <el-form :model="editingRecord" label-width="80px" size="small">
        <el-form-item label="作物" required>
          <el-select v-model="editingRecord.cropId" placeholder="请选择作物" style="width: 200px;">
            <el-option v-for="crop in crops" :key="crop.id" :label="crop.name" :value="crop.id" />
          </el-select>
        </el-form-item>
        <el-form-item label="操作" required>
          <el-input v-model="editingRecord.action" placeholder="请输入操作" style="width: 200px;"></el-input>
        </el-form-item>
        <el-form-item label="日期" required>
          <el-date-picker v-model="editingRecord.recordDate"
                          type="date"
                          placeholder="选择日期"
                          style="width: 200px;">
          </el-date-picker>
        </el-form-item>
        <el-form-item label="描述">
          <el-input v-model="editingRecord.description" placeholder="请输入描述" style="width: 200px;"></el-input>
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="saveEditedRecord">保存</el-button>
          <el-button @click="cancelEditRecord">取消</el-button>
        </el-form-item>
      </el-form>
    </div>
  </div>
</template>

<script setup>
  import { ref, onMounted } from 'vue'
  import api from '@/api'

  const records = ref([])
  const crops = ref([])
  const newRecord = ref({ cropId: null, action: '', recordDate: null, description: '' })
  const editingRecord = ref({ id: null, cropId: null, action: '', recordDate: null, description: '' })

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

  // 获取农事记录列表
  const fetchRecords = async () => {
    try {
      const res = await api.get('/farmrecords')
      const cropIds = crops.value.map(c => c.id)
      records.value = res.data
        .filter(r => cropIds.includes(r.cropId))
        .map(r => ({
          ...r,
          cropName: crops.value.find(c => c.id === r.cropId)?.name || '',
          // fetchRecords 里加一行
          recordDate: r.recordDate ? r.recordDate.slice(0, 10) : ''
        }))
    } catch (e) {
      console.error('获取农事记录列表失败', e)
      records.value = []
    }
  }

  onMounted(async () => {
    await fetchCrops()
    await fetchRecords()
  })

  // 添加农事记录
  const addRecord = async () => {
    try {
      await api.post('/farmrecords', { ...newRecord.value })
      newRecord.value = { cropId: null, action: '', recordDate: null, description: '' }
      await fetchRecords()
    } catch (e) {
      alert('添加农事记录失败')
      console.error('添加农事记录失败', e)
    }
  }

  // 编辑农事记录
  const editRecord = (record) => {
    editingRecord.value = { ...record }
  }

  // 保存编辑的记录
  const saveEditedRecord = async () => {
    if (!editingRecord.value.cropId || !editingRecord.value.action || !editingRecord.value.recordDate) {
      alert('请填写完整信息')
      return
    }
    try {
      await api.put(`/farmrecords/${editingRecord.value.id}`, { ...editingRecord.value })
      editingRecord.value = { id: null, cropId: null, action: '', recordDate: null, description: '' }
      await fetchRecords()
    }catch (e) {
      if (e.response && e.response.status === 403) {
        alert('无权为该作物编辑记录，请检查作物归属')
      } else if (e.response && e.response.data) {
        alert('保存编辑失败: ' + e.response.data)
      } else {
        alert('保存编辑失败')
      }
      console.error('保存编辑失败', e)
    }
  }

  // 取消编辑
  const cancelEditRecord = () => {
    editingRecord.value = { id: null, cropId: null, action: '', recordDate: '', description: '' }
  }

  // 删除农事记录
  const deleteRecord = async (id) => {
    if (confirm('确定要删除此记录吗？')) {
      try {
        await api.delete(`/farmrecords/${id}`)
        await fetchRecords()
      } catch (e) {
        if (e.response && e.response.data) {
          alert('删除农事记录失败: ' + e.response.data)
        } else {
          alert('删除农事记录失败')
        }
        console.error('删除农事记录失败', e)
      }
    }
  }
</script>


