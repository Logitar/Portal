<template>
  <div>
    <b-alert v-if="message" dismissible :variant="resultVariant" v-model="showResult">
      <strong>{{ $t('templates.statusFormat', { status }) }}</strong>
      {{ $t(`templates.status.${status}`) }}
      <b-link :href="`/messages/${message.id}`">{{ $t('templates.result.viewDetail') }}</b-link>
    </b-alert>
    <div class="my-2">
      <icon-button :disabled="loading" icon="paper-plane" :loading="loading" text="templates.sendToMe" variant="primary" @click="sendDemo" />
    </div>
    <h3 v-t="'templates.variables.label'" />
    <div class="my-2">
      <icon-button icon="plus" text="templates.variables.add" variant="success" @click="addVariable" />
    </div>
    <b-row v-for="(variable, index) in variables" :key="index">
      <form-field
        class="col"
        :id="`key_${index}`"
        placeholder="templates.variables.keyPlaceholder"
        :value="variable.key"
        @input="setVariable(index, $event, variable.value)"
      />
      <form-field
        class="col"
        :id="`key_${index}`"
        placeholder="templates.variables.valuePlaceholder"
        :value="variable.value"
        @input="setVariable(index, variable.key, $event)"
      >
        <icon-button class="mx-3" icon="times" variant="danger" @click="removeVariable(index)" />
      </form-field>
    </b-row>
  </div>
</template>

<script>
import Vue from 'vue'
import { sendDemoMessage } from '@/api/messages'

export default {
  name: 'DemoForm',
  props: {
    template: {
      type: Object,
      required: true
    }
  },
  data() {
    return {
      loading: false,
      message: null,
      showResult: false,
      variables: []
    }
  },
  computed: {
    resultVariant() {
      switch (this.status) {
        case 'Failed':
          return 'danger'
        case 'Sent':
          return 'success'
        case 'Unsent':
          return 'warning'
      }
      return ''
    },
    status() {
      return this.message.succeeded ? 'Sent' : this.message.hasErrors ? 'Failed' : 'Unsent'
    }
  },
  methods: {
    addVariable() {
      this.variables.push({ key: null, value: null })
    },
    removeVariable(index) {
      Vue.delete(this.variables, index)
    },
    setVariable(index, key, value) {
      Vue.set(this.variables, index, { key, value })
    },
    async sendDemo() {
      if (!this.loading) {
        this.loading = true
        try {
          const { data } = await sendDemoMessage({
            templateId: this.template.id,
            variables: this.variables.filter(({ key }) => Boolean(key))
          })
          this.message = data
          this.showResult = true
        } catch (e) {
          this.handleError(e)
        } finally {
          this.loading = false
        }
      }
    }
  }
}
</script>
