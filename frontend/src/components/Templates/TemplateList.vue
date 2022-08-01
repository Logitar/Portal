<template>
  <b-container>
    <h1 v-t="'templates.title'" />
    <div class="my-2">
      <icon-button class="mx-1" icon="sync-alt" :loading="loading" text="actions.refresh" variant="primary" @click="refresh()" />
      <icon-button class="mx-1" :href="createUrl" icon="plus" text="actions.create" variant="success" />
    </div>
    <b-row>
      <search-field class="col" v-model="search" />
      <realm-select class="col" v-model="realm" />
      <sort-select class="col" :desc="desc" :options="sortOptions" v-model="sort" @desc="desc = $event" />
      <count-select class="col" v-model="count" />
    </b-row>
    <template v-if="templates.length">
      <table id="table" class="table table-striped">
        <thead>
          <tr>
            <th scope="col" v-t="'templates.key.label'" />
            <th scope="col" v-t="'templates.displayName.label'" />
            <th scope="col" v-t="'templates.contentType.label'" />
            <th scope="col" v-t="'updatedAt'" />
            <th scope="col" />
          </tr>
        </thead>
        <tbody>
          <tr v-for="template in templates" :key="template.id">
            <td>
              <b-link :href="`/templates/${template.id}`">{{ template.key }}</b-link>
            </td>
            <td v-text="template.displayName || '—'" />
            <td>{{ $t(`templates.contentType.options.${template.contentType}`) }}</td>
            <td>{{ $d(new Date(template.updatedAt || template.createdAt), 'medium') }}</td>
            <td>
              <icon-button icon="trash-alt" text="actions.delete" variant="danger" v-b-modal="`delete_${template.id}`" />
              <delete-modal
                confirm="templates.delete.confirm"
                :displayName="template.displayName ? `${template.displayName} (${template.key})` : template.key"
                :id="`delete_${template.id}`"
                :loading="loading"
                title="templates.delete.title"
                @ok="onDelete(template, $event)"
              />
            </td>
          </tr>
        </tbody>
      </table>
      <b-pagination v-model="page" :total-rows="total" :per-page="count" aria-controls="table" />
    </template>
    <p v-else v-t="'templates.empty'" />
  </b-container>
</template>

<script>
import RealmSelect from '@/components/Realms/RealmSelect.vue'
import { deleteTemplate, getTemplates } from '@/api/templates'
import { getQueryString } from '@/helpers/queryUtils'

export default {
  name: 'TemplateList',
  components: {
    RealmSelect
  },
  data() {
    return {
      count: 10,
      desc: false,
      loading: false,
      page: 1,
      realm: null,
      search: null,
      sort: 'DisplayName',
      templates: [],
      total: 0
    }
  },
  computed: {
    createUrl() {
      return '/create-template' + getQueryString({ realm: this.realm })
    },
    params() {
      return {
        realm: this.realm,
        search: this.search,
        sort: this.sort,
        desc: this.desc,
        index: (this.page - 1) * this.count,
        count: this.count
      }
    },
    sortOptions() {
      return this.orderBy(
        Object.entries(this.$i18n.t('templates.sort.options')).map(([value, text]) => ({ text, value })),
        'text'
      )
    }
  },
  methods: {
    async onDelete({ id }, callback = null) {
      if (!this.loading) {
        this.loading = true
        let refresh = false
        try {
          await deleteTemplate(id)
          refresh = true
          this.toast('success', 'templates.delete.success')
          if (typeof callback === 'function') {
            callback()
          }
        } catch (e) {
          this.handleError(e)
        } finally {
          this.loading = false
        }
        if (refresh) {
          await this.refresh()
        }
      }
      if (callback) {
        callback()
      }
    },
    async refresh(params = null) {
      if (!this.loading) {
        this.loading = true
        try {
          const { data } = await getTemplates(params ?? this.params)
          this.templates = data.items
          this.total = data.total
        } catch (e) {
          this.handleError(e)
        } finally {
          this.loading = false
        }
      }
    }
  },
  watch: {
    params: {
      deep: true,
      immediate: true,
      async handler(newValue, oldValue) {
        if (newValue?.index && oldValue && (newValue.realm !== oldValue.realm || newValue.search !== oldValue.search || newValue.count !== oldValue.count)) {
          this.page = 1
          await this.refresh()
        } else {
          await this.refresh(newValue)
        }
      }
    }
  }
}
</script>
