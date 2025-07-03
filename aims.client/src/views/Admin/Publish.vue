<template>
  <el-card>
    <h2>内容发布</h2>
    <el-form :model="form" label-width="80px">
      <el-form-item label="标题">
        <el-input v-model="form.title" />
      </el-form-item>
      <el-form-item label="内容">
        <el-input type="textarea" v-model="form.content" />
      </el-form-item>
      <el-form-item label="日期">
        <el-date-picker v-model="form.publishDate"
                        type="date"
                        placeholder="选择日期"
                        style="width: 200px;">
        </el-date-picker>
      </el-form-item>
      <el-form-item>
        <el-button type="primary" @click="publish">发布</el-button>
      </el-form-item>
      <el-alert v-if="success" type="success" show-icon :closable="false" title="发布成功" />
    </el-form>
  </el-card>
</template>

<script setup>
  import { ref } from 'vue'
  import api from '@/api'

  const form = ref({ title: '', content: '', publishDate: null })
  const success = ref(false)

  const publish = async () => {
    await api.post('/policies', form.value)
    success.value = true
    form.value = { title: '', content: '', publishDate: null }
    setTimeout(() => (success.value = false), 2000)
  }
</script>
